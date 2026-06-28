using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Domain.Events;

public sealed record OrdenCancelada(
    OrdenId OrdenId,
    string Motivo,
    DateTime FechaCancelacion) : IDomainEvent
{
    public static OrdenCancelada Crear(OrdenId ordenId, string motivo) =>
        new(ordenId, motivo, DateTime.UtcNow);
}
