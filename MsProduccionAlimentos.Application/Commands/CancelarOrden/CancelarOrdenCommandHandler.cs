using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.CancelarOrden;

public class CancelarOrdenCommandHandler : IRequestHandler<CancelarOrdenCommand, Unit>
{
    private readonly IOrdenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarOrdenCommandHandler(IOrdenRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _repository.ObtenerPorId(OrdenId.De(request.OrdenId))
            ?? throw new InvalidOperationException($"No se encontró la orden con Id '{request.OrdenId}'.");

        orden.Cancelar(request.Motivo);

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
