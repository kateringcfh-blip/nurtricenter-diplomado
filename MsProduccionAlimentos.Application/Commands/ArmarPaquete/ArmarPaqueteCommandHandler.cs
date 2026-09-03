using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.ArmarPaquete;

public class ArmarPaqueteCommandHandler : IRequestHandler<ArmarPaqueteCommand, Guid>
{
    private readonly IPaqueteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ArmarPaqueteCommandHandler(IPaqueteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(ArmarPaqueteCommand request, CancellationToken cancellationToken)
    {
        var etiqueta = Etiqueta.Crear(request.NombrePaciente, request.DireccionEntrega, request.NumeroId);
        var ordenId = OrdenId.De(request.OrdenId);

        var paquete = Paquete.Armar(request.PacienteId, ordenId, etiqueta, request.Fecha);

        await _repository.Agregar(paquete);
        await _unitOfWork.GuardarCambios(cancellationToken);

        return paquete.Id.Valor;
    }
}
