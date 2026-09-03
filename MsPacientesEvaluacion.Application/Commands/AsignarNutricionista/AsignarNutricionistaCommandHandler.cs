using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.AsignarNutricionista;

public class AsignarNutricionistaCommandHandler : IRequestHandler<AsignarNutricionistaCommand, Unit>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AsignarNutricionistaCommandHandler(IPacienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AsignarNutricionistaCommand request, CancellationToken cancellationToken)
    {
        var pacienteId = PacienteId.De(request.PacienteId);
        var paciente = await _repository.ObtenerPorId(pacienteId)
            ?? throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

        paciente.AsignarNutricionista(request.NutricionistaId);

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
