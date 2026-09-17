using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MsPacientesEvaluacion.Infrastructure.Persistence;

namespace MsPacientesEvaluacion.ContractTests;

[ApiController]
[Route("provider-states")]
public class ProviderStateController : ControllerBase
{
    private readonly PacientesDbContext _context;

    public ProviderStateController(PacientesDbContext context)
    {
        _context = context;
    }

    [HttpPost("")]
    public async Task<IActionResult> SetProviderState([FromBody] ProviderStateDto state)
    {
        switch (state.State)
        {
            case "un paciente con id 11111111-1111-1111-1111-111111111111 existe":
                await SembrarPacienteExistente();
                break;
            case "el paciente 99999999-9999-9999-9999-999999999999 no existe":
                // No hacer nada — la base de datos no tiene ese paciente
                break;
        }

        return Ok();
    }

    private async Task SembrarPacienteExistente()
    {
        // Limpiar tabla primero para evitar duplicados
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Pacientes");

        // Insertar directamente via SQL para poder fijar el Id al Guid que el contrato espera.
        // Paciente.Registrar() genera el PacienteId internamente (PacienteId.Crear()),
        // por lo que no es posible especificar un Guid fijo usando el dominio.
        // Solo se insertan las columnas requeridas (no nulas); ConsultaInicial y Anamnesis son opcionales.
        await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Pacientes (
                Id,
                Nombre,
                Apellido,
                FechaNacimiento,
                NutricionistaId,
                Estado,
                Contacto_Email,
                Contacto_Direccion,
                Contacto_Telefono
            ) VALUES (
                '11111111-1111-1111-1111-111111111111',
                'Carlos',
                'Perez',
                '1985-03-15',
                'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
                'Activo',
                'carlos.perez@mail.com',
                'Av. Principal 123',
                '555-0001'
            )");
    }
}

public record ProviderStateDto(string State, Dictionary<string, string>? Params);
