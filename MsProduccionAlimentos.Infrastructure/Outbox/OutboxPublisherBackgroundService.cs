using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Infrastructure.Persistence;

namespace MsProduccionAlimentos.Infrastructure.Outbox;

/// <summary>
/// Segunda mitad del patrón Transactional Outbox: cada pocos segundos revisa la tabla
/// OutboxMessages en busca de mensajes sin publicar (ProcesadoEn == null), los envía a
/// RabbitMQ a través del mismo IEventPublisher ya usado por el resto de la integración,
/// y los marca como procesados. Si falla la publicación de uno, simplemente se reintenta
/// en el siguiente ciclo (entrega "al menos una vez").
/// </summary>
public sealed class OutboxPublisherBackgroundService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(3);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;

    public OutboxPublisherBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxPublisherBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublicarPendientesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                TryLogWarning(ex, "[Outbox] Error al procesar mensajes pendientes.");
            }

            try
            {
                await Task.Delay(Intervalo, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Apagado normal del servicio.
            }
        }
    }

    private async Task PublicarPendientesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var pendientes = await context.OutboxMessages
            .Where(m => m.ProcesadoEn == null)
            .OrderBy(m => m.OcurridoEn)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (pendientes.Count == 0)
            return;

        foreach (var mensaje in pendientes)
        {
            try
            {
                await publisher.PublishRawAsync(mensaje.RoutingKey, mensaje.Payload, stoppingToken);
                mensaje.ProcesadoEn = DateTime.UtcNow;

                _logger.LogInformation(
                    "[Outbox] Publicado {Tipo} (routingKey={RoutingKey}, outboxId={Id}).",
                    mensaje.Tipo, mensaje.RoutingKey, mensaje.Id);
            }
            catch (Exception ex)
            {
                TryLogWarning(
                    ex,
                    $"[Outbox] No se pudo publicar el mensaje {mensaje.Id} ({mensaje.RoutingKey}); se reintentará en el siguiente ciclo.");
            }
        }

        await context.SaveChangesAsync(stoppingToken);
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
