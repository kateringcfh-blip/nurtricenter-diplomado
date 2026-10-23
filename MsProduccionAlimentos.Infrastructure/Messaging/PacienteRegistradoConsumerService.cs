using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Application.IntegrationEvents;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MsProduccionAlimentos.Infrastructure.Messaging;

/// <summary>
/// Escucha en RabbitMQ el evento de integración "paciente.registrado", publicado por
/// ms-pacientes-evaluacion, y lo registra (log + store en memoria) para poder
/// demostrar la integración real entre los dos microservicios.
/// AutomaticRecoveryEnabled queda en false a propósito: ya manejamos el reintento inicial
/// nosotros mismos, y la recuperación automática de RabbitMQ.Client 6.x tiene un bug conocido
/// de concurrencia al hacer Dispose() de canales/conexiones rápidamente (como en tests).
/// </summary>
public sealed class PacienteRegistradoConsumerService : BackgroundService
{
    private const string QueueName = "produccion.paciente-registrado";
    private const string RoutingKey = "paciente.registrado";

    private readonly RabbitMqOptions _options;
    private readonly IReceivedIntegrationEventsStore _store;
    private readonly ILogger<PacienteRegistradoConsumerService> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public PacienteRegistradoConsumerService(
        IOptions<RabbitMqOptions> options,
        IReceivedIntegrationEventsStore store,
        ILogger<PacienteRegistradoConsumerService> logger)
    {
        _options = options.Value;
        _store = store;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = false,
        };

        _connection = CreateConnectionWithRetry(factory);
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: QueueName, exchange: _options.Exchange, routingKey: RoutingKey);

        _logger.LogInformation(
            "Consumidor de '{RoutingKey}' conectado a RabbitMQ en {HostName}:{Port}, cola '{QueueName}'.",
            RoutingKey, _options.HostName, _options.Port, QueueName);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            return Task.CompletedTask;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, eventArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var evento = JsonSerializer.Deserialize<PacienteRegistradoIntegrationEvent>(json);

                _logger.LogInformation(
                    "[Integración RabbitMQ] PacienteRegistrado recibido <- ms-pacientes-evaluacion | PacienteId={PacienteId} Nombre={Nombre} FechaRegistro={FechaRegistro}",
                    evento?.PacienteId, evento?.Nombre, evento?.FechaRegistro);

                if (evento is not null)
                {
                    _store.Registrar(nameof(PacienteRegistradoIntegrationEvent), RoutingKey, evento);
                }

                _channel!.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje de la cola '{QueueName}'.", QueueName);
                _channel!.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private IConnection CreateConnectionWithRetry(ConnectionFactory factory, int maxRetries = 10, int delaySeconds = 3)
    {
        for (var intento = 1; intento <= maxRetries; intento++)
        {
            try
            {
                return factory.CreateConnection();
            }
            catch (Exception ex) when (intento < maxRetries)
            {
                _logger.LogWarning(
                    "No se pudo conectar a RabbitMQ en {HostName}:{Port} (intento {Intento}/{MaxRetries}): {Mensaje}. Reintentando en {Delay}s...",
                    factory.HostName, factory.Port, intento, maxRetries, ex.Message, delaySeconds);
                Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        return factory.CreateConnection();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
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
            TryLogWarning(ex, "Error no crítico al cerrar la conexión de RabbitMQ.");
        }

        await base.StopAsync(cancellationToken);
    }

    private void TryLogWarning(Exception ex, string mensaje)
    {
        try
        {
            _logger.LogWarning(ex, mensaje);
        }
        catch
        {
            // El logger (p. ej. el proveedor EventLog en Windows) puede estar ya liberado
            // durante el apagado del host; no hay nada más que hacer con esto.
        }
    }
}
