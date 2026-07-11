namespace MsProduccionAlimentos.Application.DTOs;

public record PaqueteDto(
    Guid Id,
    Guid PacienteId,
    Guid OrdenId,
    DateOnly Fecha,
    string Estado,
    string NombrePaciente,
    int TotalPorciones);
