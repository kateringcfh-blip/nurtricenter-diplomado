using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.EnvasarPorcion;

public class EnvasarPorcionCommandHandler : IRequestHandler<EnvasarPorcionCommand, Unit>
{
    private readonly IPaqueteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public EnvasarPorcionCommandHandler(IPaqueteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EnvasarPorcionCommand request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.ObtenerPorId(PaqueteId.De(request.PaqueteId))
            ?? throw new InvalidOperationException($"No se encontró el paquete con Id '{request.PaqueteId}'.");

        var porcion = paquete.Porciones.FirstOrDefault(p => p.Id.Valor == request.PorcionId)
            ?? throw new InvalidOperationException($"No se encontró la porción con Id '{request.PorcionId}' en el paquete.");

        porcion.Envasar();

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
