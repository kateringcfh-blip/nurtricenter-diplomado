using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarEvaluacion;

public class RegistrarEvaluacionCommandHandler : IRequestHandler<RegistrarEvaluacionCommand, Unit>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarEvaluacionCommandHandler(IPacienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RegistrarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var pacienteId = PacienteId.De(request.PacienteId);
        var paciente = await _repository.ObtenerPorId(pacienteId)
            ?? throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

        var medidas = Medidas.Crear(request.Cintura, request.Cadera, request.Imc);
        var adherencia = Enum.Parse<NivelAdherencia>(request.Adherencia, ignoreCase: true);
        
        paciente.RegistrarEvaluacion(request.Fecha, request.Peso, medidas, request.Observaciones, adherencia, request.PlanId);

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
