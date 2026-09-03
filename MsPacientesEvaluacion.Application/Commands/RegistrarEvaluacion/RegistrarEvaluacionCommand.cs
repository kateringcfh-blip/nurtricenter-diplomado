using MediatR;
using MsPacientesEvaluacion.Domain.Enums;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarEvaluacion;

public record RegistrarEvaluacionCommand(
    Guid PacienteId,
    DateOnly Fecha,
    decimal Peso,
    decimal Cintura,
    decimal Cadera,
    decimal Imc,
    string Observaciones,
    string Adherencia,
    Guid PlanId) : IRequest<Unit>;
