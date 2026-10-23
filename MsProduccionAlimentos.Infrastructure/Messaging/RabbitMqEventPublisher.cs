using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MsProduccionAlimentos.Application.Interfaces;
using RabbitMQ.Client;

namespace MsProduccionAlimentos.Infrastructure.Messaging;

/// <summary>
/// Publica eventos de integración en un exchange tipo "topic" de RabbitMQ.
/// Se registra como Singleton: mantiene una única conexión/canal durante la vida de la app.
/// Si RabbitMQ todavía no está listo al arrancar (ej. el contenedor Docker aún no respondió
/// el healthcheck), reintenta la conexión con backoff antes de rendirse.
/// AutomaticRecoveryEnabled queda en false a propósito: ya manejamos el reintento inicial
/// nosotros mismos, y la recuperación automática de RabbitMQ.Client 6.x tiene un bug conocido
/// de concurrencia al hacer Dispose() de canales/conexiones rápidamente (como en tests).
/// </summary>
public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly object _publishLock = new();

    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = false,
        };

        _connection = CreateConnectionWithRetry(factory, logger);
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync(string routingKey, object integrationEvent, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());
        return PublishRawAsync(routingKey, json, cancellationToken);
    }

    public Task PublishRawAsync(string routingKey, string jsonPayload, CancellationToken cancellationToken = default)
    {
        var body = Encoding.UTF8.GetBytes(jsonPayload);

        lock (_publishLock)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: _options.Exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }

        return Task.CompletedTask;
    }

    private static IConnection CreateConnectionWithRetry(
        ConnectionFactory factory, ILogger logger, int maxRetries = 10, int delaySeconds = 3)
    {
        for (var intento = 1; intento <= maxRetries; intento++)
        {
            try
            {
                return factory.CreateConnection();
            }
            catch (Exception ex) when (intento < maxRetries)
            {
                logger.LogWarning(
                    "No se pudo conectar a RabbitMQ en {HostName}:{Port} (intento {Intento}/{MaxRetries}): {Mensaje}. Reintentando en {Delay}s...",
                    factory.HostName, factory.Port, intento, maxRetries, ex.Message, delaySeconds);
                Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        // Último intento: si falla, se deja propagar la excepción.
        return factory.CreateConnection();
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            // Solo puede fallar durante el cierre ordenado; no debe tumbar el shutdown de la app.
            // El propio logger también puede estar cerrándose en ese momento (p. ej. en tests de
            // integración con WebApplicationFactory), así que el log en sí va protegido aparte.
            try
            {
                _logger.LogWarning(ex, "Error no crítico al cerrar la conexión de RabbitMQ.");
            }
            catch
            {
                // El logger puede estar ya liberado durante el apagado del host; no hay nada más que hacer.
            }
        }
    }
}
