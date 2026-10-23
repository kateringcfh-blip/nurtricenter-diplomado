using System.Collections.Concurrent;
using MsProduccionAlimentos.Application.Interfaces;

namespace MsProduccionAlimentos.Infrastructure.Messaging;

/// <summary>
/// Implementación en memoria (thread-safe) del registro de eventos de integración recibidos.
/// Suficiente para la demo/defensa; no persiste entre reinicios de la API.
/// </summary>
public sealed class InMemoryReceivedIntegrationEventsStore : IReceivedIntegrationEventsStore
{
    private readonly ConcurrentQueue<ReceivedIntegrationEvent> _eventos = new();
    private const int MaxEventosGuardados = 50;

    public void Registrar(string tipoEvento, string routingKey, object payload)
    {
        _eventos.Enqueue(new ReceivedIntegrationEvent(tipoEvento, routingKey, DateTime.UtcNow, payload));

        while (_eventos.Count > MaxEventosGuardados && _eventos.TryDequeue(out _))
        {
            // descarta los más antiguos
        }
    }

    public IReadOnlyList<ReceivedIntegrationEvent> ObtenerUltimos(int cantidad = 20) =>
        _eventos.Reverse().Take(cantidad).ToList();
}
