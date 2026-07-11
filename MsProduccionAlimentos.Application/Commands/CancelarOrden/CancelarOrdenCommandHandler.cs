using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.CancelarOrden;

public class CancelarOrdenCommandHandler : IRequestHandler<CancelarOrdenCommand, Unit>
{
    private readonly IOrdenRepository _repository;

    public CancelarOrdenCommandHandler(IOrdenRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(CancelarOrdenCommand request, CancellationToken cancellationToken)
    {
        var ordenId = OrdenId.De(request.OrdenId);
        var orden = await _repository.ObtenerPorId(ordenId)
            ?? throw new InvalidOperationException($"No se encontró la orden con Id '{request.OrdenId}'.");

        orden.Cancelar(request.Motivo);

        await _repository.GuardarCambios();

        return Unit.Value;
    }
}
