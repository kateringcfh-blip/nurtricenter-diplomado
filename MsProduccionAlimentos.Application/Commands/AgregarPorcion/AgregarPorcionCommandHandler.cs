using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.AgregarPorcion;

public class AgregarPorcionCommandHandler : IRequestHandler<AgregarPorcionCommand, Guid>
{
    private readonly IPaqueteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarPorcionCommandHandler(IPaqueteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AgregarPorcionCommand request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.ObtenerPorId(PaqueteId.De(request.PaqueteId))
            ?? throw new InvalidOperationException($"No se encontró el paquete con Id '{request.PaqueteId}'.");

        var porcion = paquete.AgregarPorcion(request.RecetaId, request.Cantidad);
        await _unitOfWork.GuardarCambios(cancellationToken);
        return porcion.Id.Valor;
    }
}
