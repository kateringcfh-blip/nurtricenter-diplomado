using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Entities;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarConsultaInicial;

public class RegistrarConsultaInicialCommandHandler : IRequestHandler<RegistrarConsultaInicialCommand, Unit>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarConsultaInicialCommandHandler(IPacienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RegistrarConsultaInicialCommand request, CancellationToken cancellationToken)
    {
        var pacienteId = PacienteId.De(request.PacienteId);
        var paciente = await _repository.ObtenerPorId(pacienteId)
            ?? throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

        var anamnesis = Anamnesis.Registrar(
            request.HabitosAlimenticios,
            request.AntecedentesClinicos,
            request.NecesidadesEspecificas);

        paciente.RegistrarConsultaInicial(request.Fecha, request.Peso, request.Altura, anamnesis);

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
