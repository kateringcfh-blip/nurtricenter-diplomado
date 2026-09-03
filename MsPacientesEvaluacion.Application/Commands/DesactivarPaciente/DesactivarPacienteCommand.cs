using MediatR;

namespace MsPacientesEvaluacion.Application.Commands.DesactivarPaciente;

public record DesactivarPacienteCommand(Guid PacienteId) : IRequest<Unit>;
