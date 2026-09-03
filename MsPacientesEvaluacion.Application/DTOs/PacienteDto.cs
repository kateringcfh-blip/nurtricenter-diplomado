namespace MsPacientesEvaluacion.Application.DTOs;

public record PacienteDto(
    Guid Id,
    string Nombre,
    string Apellido,
    string Estado,
    int TotalEvaluaciones);
