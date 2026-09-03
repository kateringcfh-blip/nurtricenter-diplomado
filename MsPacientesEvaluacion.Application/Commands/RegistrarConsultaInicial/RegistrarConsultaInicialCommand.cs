using MediatR;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarConsultaInicial;

public record RegistrarConsultaInicialCommand(
    Guid PacienteId,
    DateOnly Fecha,
    decimal Peso,
    decimal Altura,
    string HabitosAlimenticios,
    string AntecedentesClinicos,
    string NecesidadesEspecificas) : IRequest<Unit>;
