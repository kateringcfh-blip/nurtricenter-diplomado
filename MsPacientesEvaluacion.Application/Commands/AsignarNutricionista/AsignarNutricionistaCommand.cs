using MediatR;

namespace MsPacientesEvaluacion.Application.Commands.AsignarNutricionista;

public record AsignarNutricionistaCommand(
    Guid PacienteId,
    Guid NutricionistaId) : IRequest<Unit>;
