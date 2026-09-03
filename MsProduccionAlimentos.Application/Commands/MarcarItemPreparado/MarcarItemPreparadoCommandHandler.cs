using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.MarcarItemPreparado;

public class MarcarItemPreparadoCommandHandler : IRequestHandler<MarcarItemPreparadoCommand, Unit>
{
    private readonly IOrdenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarItemPreparadoCommandHandler(IOrdenRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(MarcarItemPreparadoCommand request, CancellationToken cancellationToken)
    {
        var orden = await _repository.ObtenerPorId(OrdenId.De(request.OrdenId))
            ?? throw new InvalidOperationException($"No se encontró la orden con Id '{request.OrdenId}'.");

        orden.MarcarItemPreparado(ItemOrdenId.De(request.ItemOrdenId), request.Cantidad);

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
