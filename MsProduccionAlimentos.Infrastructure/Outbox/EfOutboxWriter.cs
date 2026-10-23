using System.Text.Json;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Infrastructure.Persistence;

namespace MsProduccionAlimentos.Infrastructure.Outbox;

/// <summary>
/// Encola el evento de integración en el change tracker del mismo NurTricenterDbContext
/// que usa el handler (son el mismo scope). No llama SaveChanges: el mensaje queda
/// pendiente hasta que el handler confirme la transacción completa (aggregate + outbox)
/// con _unitOfWork.GuardarCambios().
/// </summary>
public sealed class EfOutboxWriter : IIntegrationEventOutbox
{
    private readonly NurTricenterDbContext _context;

    public EfOutboxWriter(NurTricenterDbContext context)
    {
        _context = context;
    }

    public void Enqueuear(string routingKey, object integrationEvent)
    {
        var mensaje = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Tipo = integrationEvent.GetType().Name,
            RoutingKey = routingKey,
            Payload = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
            OcurridoEn = DateTime.UtcNow,
            ProcesadoEn = null,
        };

        _context.OutboxMessages.Add(mensaje);
    }
}
