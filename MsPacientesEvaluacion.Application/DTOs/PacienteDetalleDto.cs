namespace MsPacientesEvaluacion.Application.DTOs;

public record PacienteDetalleDto(
    Guid Id,
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Estado,
    string Email,
    string Telefono,
    bool TieneConsultaInicial,
    int TotalEvaluaciones);
