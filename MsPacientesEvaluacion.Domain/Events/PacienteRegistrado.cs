using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Domain.Events;

public sealed record PacienteRegistrado(
    PacienteId PacienteId,
    string Nombre,
    DateTime FechaRegistro) : IDomainEvent
{
    public static PacienteRegistrado Crear(PacienteId pacienteId, string nombre) =>
        new(pacienteId, nombre, DateTime.UtcNow);
}
