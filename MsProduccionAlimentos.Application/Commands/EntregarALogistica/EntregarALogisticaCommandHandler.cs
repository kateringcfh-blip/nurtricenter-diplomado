using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.EntregarALogistica;

public class EntregarALogisticaCommandHandler : IRequestHandler<EntregarALogisticaCommand, Unit>
{
    private readonly IPaqueteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public EntregarALogisticaCommandHandler(IPaqueteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EntregarALogisticaCommand request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.ObtenerPorId(PaqueteId.De(request.PaqueteId))
            ?? throw new InvalidOperationException($"No se encontró el paquete con Id '{request.PaqueteId}'.");

        paquete.MarcarListo();
        paquete.EntregarALogistica();

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
