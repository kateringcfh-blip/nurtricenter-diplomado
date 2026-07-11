using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.EntregarALogistica;

public class EntregarALogisticaCommandHandler : IRequestHandler<EntregarALogisticaCommand, Unit>
{
    private readonly IPaqueteRepository _repository;

    public EntregarALogisticaCommandHandler(IPaqueteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(EntregarALogisticaCommand request, CancellationToken cancellationToken)
    {
        var paqueteId = PaqueteId.De(request.PaqueteId);
        var paquete = await _repository.ObtenerPorId(paqueteId)
            ?? throw new InvalidOperationException($"No se encontró el paquete con Id '{request.PaqueteId}'.");

        paquete.MarcarListo();
        paquete.EntregarALogistica();

        await _repository.GuardarCambios();

        return Unit.Value;
    }
}
