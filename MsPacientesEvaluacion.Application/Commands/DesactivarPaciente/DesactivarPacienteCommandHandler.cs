using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.DesactivarPaciente;

public class DesactivarPacienteCommandHandler : IRequestHandler<DesactivarPacienteCommand, Unit>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DesactivarPacienteCommandHandler(IPacienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DesactivarPacienteCommand request, CancellationToken cancellationToken)
    {
        var pacienteId = PacienteId.De(request.PacienteId);
        var paciente = await _repository.ObtenerPorId(pacienteId)
            ?? throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

        paciente.Desactivar();

        await _unitOfWork.GuardarCambios(cancellationToken);
        return Unit.Value;
    }
}
