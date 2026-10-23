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
    private readonly IEventPublisher _eventPublisher;

    public CancelarOrdenCommandHandler(
        IOrdenRepository repository,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(CancelarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _repository.ObtenerPorId(OrdenId.De(request.OrdenId))
            ?? throw new InvalidOperationException($"No se encontró la orden con Id '{request.OrdenId}'.");

        orden.Cancelar(request.Motivo);

        await _unitOfWork.GuardarCambios(cancellationToken);

        // Publicar eventos de integración después de confirmar la persistencia,
        // para no notificar a otros microservicios si el guardado falla.
        foreach (var evento in orden.EventosOcurridos)
        {
            if (evento is OrdenCancelada ordenCancelada)
            {
                var integrationEvent = new OrdenCanceladaIntegrationEvent(
                    ordenCancelada.OrdenId.Valor,
                    ordenCancelada.Motivo,
                    ordenCancelada.FechaCancelacion);

                await _eventPublisher.PublishAsync("orden.cancelada", integrationEvent, cancellationToken);
            }
        }
        orden.LimpiarEventos();

        return Unit.Value;
    }
}
