using MsPacientesEvaluacion.Domain.Enums;

namespace MsPacientesEvaluacion.Api.Controllers;

public record RegistrarConsultaInicialRequest(
    DateOnly Fecha,
    decimal Peso,
    decimal Altura,
    string HabitosAlimenticios,
    string AntecedentesClinicos,
    string NecesidadesEspecificas);

public record RegistrarEvaluacionRequest(
    DateOnly Fecha,
    decimal Peso,
    decimal Cintura,
    decimal Cadera,
    decimal Imc,
    string Observaciones,
    string Adherencia,
    Guid PlanId);

public record AsignarNutricionistaRequest(Guid NutricionistaId);
