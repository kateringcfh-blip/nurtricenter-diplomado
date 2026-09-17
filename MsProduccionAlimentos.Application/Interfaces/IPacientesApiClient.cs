namespace MsProduccionAlimentos.Application.Interfaces;

public record PacienteInfoDto(
    Guid Id,
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Estado,
    string Email,
    string Telefono,
    bool TieneConsultaInicial,
    int TotalEvaluaciones);

public interface IPacientesApiClient
{
    Task<PacienteInfoDto?> ObtenerPaciente(Guid pacienteId, CancellationToken cancellationToken = default);
}
