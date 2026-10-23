using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Application.IntegrationEvents;
using MsProduccionAlimentos.Domain.Events;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.CancelarOrden;

public class CancelarOrdenCommandHandler : IRequestHandler<CancelarOrdenCommand, Unit>
{
    private readonly IOrdenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventOutbox _outbox;

    public CancelarOrdenCommandHandler(
        IOrdenRepository repository,
        IUnitOfWork unitOfWork,
        IIntegrationEventOutbox outbox)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
    }

    public async Task<Unit> Handle(CancelarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _repository.ObtenerPorId(OrdenId.De(request.OrdenId))
            ?? throw new InvalidOperationException($"No se encontró la orden con Id '{request.OrdenId}'.");

        orden.Cancelar(request.Motivo);

        // Patrón Transactional Outbox: el evento de integración se encola en el mismo
        // DbContext que el aggregate, así que se persiste atómicamente junto con la
        // orden en el GuardarCambios() de abajo. La publicación real a RabbitMQ la hace,
        // por separado, el OutboxPublisherBackgroundService.
        foreach (var evento in orden.EventosOcurridos)
        {
            if (evento is OrdenCancelada ordenCancelada)
            {
                var integrationEvent = new OrdenCanceladaIntegrationEvent(
                    ordenCancelada.OrdenId.Valor,
                    ordenCancelada.Motivo,
                    ordenCancelada.FechaCancelacion);

                _outbox.Enqueuear("orden.cancelada", integrationEvent);
            }
        }
        orden.LimpiarEventos();

        await _unitOfWork.GuardarCambios(cancellationToken);

        return Unit.Value;
    }
}
