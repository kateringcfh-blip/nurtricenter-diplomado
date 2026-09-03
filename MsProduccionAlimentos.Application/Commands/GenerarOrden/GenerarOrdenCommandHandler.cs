using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Application.Commands.GenerarOrden;

public class GenerarOrdenCommandHandler : IRequestHandler<GenerarOrdenCommand, Guid>
{
    private readonly IOrdenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerarOrdenCommandHandler(IOrdenRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(GenerarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = OrdenProduccion.Generar(
            request.EncargadoCocinaId,
            request.LoteProduccion,
            request.Fecha);

        foreach (var item in request.Items)
            orden.AgregarItem(item.RecetaId, item.CantidadRequerida);

        await _repository.Agregar(orden);
        await _unitOfWork.GuardarCambios(cancellationToken);

        return orden.Id.Valor;
    }
}
