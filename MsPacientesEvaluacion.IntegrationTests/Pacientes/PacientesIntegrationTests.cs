using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Infrastructure.Persistence;
using MsPacientesEvaluacion.IntegrationTests.Infrastructure;
using Xunit;

namespace MsPacientesEvaluacion.IntegrationTests.Pacientes;

public class PacientesIntegrationTests : IClassFixture<PacientesApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly PacientesApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public PacientesIntegrationTests(PacientesApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // -----------------------------------------------------------------------
    // Helper: crea un paciente y devuelve su id
    // -----------------------------------------------------------------------
    private async Task<Guid> CrearPacienteAsync(string nombre = "Luis", string apellido = "Ramírez")
    {
        var request = new
        {
            nombre,
            apellido,
            fechaNacimiento = "1990-05-15",
            email = $"{nombre.ToLower()}.{apellido.ToLower()}@mail.com",
            direccion = "Av. Las Flores 789",
            telefono = "555-0001",
            nutricionistaId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/pacientes", request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<CreatedIdDto>(JsonOptions);
        return body!.Id;
    }

    // -----------------------------------------------------------------------
    // Test 1 — Flujo correcto: registrar paciente
    // -----------------------------------------------------------------------
    [Fact]
    public async Task RegistrarPaciente_ConDatosCompletos_Retorna201ConId()
    {
        // Arrange
        var nutricionistaId = Guid.NewGuid();
        var request = new
        {
            nombre = "María",
            apellido = "López",
            fechaNacimiento = "1985-03-20",
            email = "maria.lopez@mail.com",
            direccion = "Calle Mayor 101",
            telefono = "555-9999",
            nutricionistaId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pacientes", request);

        // Assert — HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — Body
        var body = await response.Content.ReadFromJsonAsync<CreatedIdDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);

        // Assert — Persistencia (ToList en memoria para evitar limitación de traducción de SQLite con Value Objects)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PacientesDbContext>();

        var todosPacientes = await context.Pacientes.ToListAsync();
        var paciente = todosPacientes.FirstOrDefault(p => p.Id.Valor == body.Id);
        Assert.NotNull(paciente);
        Assert.Equal("María", paciente.Nombre);
        Assert.Equal("López", paciente.Apellido);
        Assert.Equal(EstadoPaciente.Activo, paciente.Estado);
        Assert.Null(paciente.ConsultaInicial);
    }

    // -----------------------------------------------------------------------
    // Test 2 — Flujo correcto encadenado: registrar consulta inicial
    // -----------------------------------------------------------------------
    [Fact]
    public async Task RegistrarConsultaInicial_PacienteExistente_Retorna204YCambiaEstado()
    {
        // Arrange: primero crear el paciente
        var pacienteId = await CrearPacienteAsync("Pedro", "Soto");

        var requestConsulta = new
        {
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            peso = 75.5m,
            altura = 1.78m,
            habitosAlimenticios = "Dieta mediterránea, 3 comidas al día",
            antecedentesClinicos = "Hipertensión controlada",
            necesidadesEspecificas = "Reducción de sodio"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/pacientes/{pacienteId}/consulta-inicial", requestConsulta);

        // Assert — HTTP
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert — Persistencia (ToList en memoria para evitar limitación de traducción de SQLite con Value Objects)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PacientesDbContext>();

        var todosPacientes = await context.Pacientes.ToListAsync();
        var paciente = todosPacientes.FirstOrDefault(p => p.Id.Valor == pacienteId);
        Assert.NotNull(paciente);
        Assert.NotNull(paciente.ConsultaInicial);
        Assert.Equal(EstadoPaciente.EnEvaluacion, paciente.Estado);
        Assert.Equal(75.5m, paciente.ConsultaInicial.Peso);
    }

    // -----------------------------------------------------------------------
    // Test 3 — Flujo incorrecto: paciente inexistente
    // -----------------------------------------------------------------------
    [Fact]
    public async Task RegistrarConsultaInicial_PacienteInexistente_Retorna404()
    {
        // Arrange: id que no existe en la base de datos
        var idInexistente = Guid.NewGuid();

        var requestConsulta = new
        {
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            peso = 70.0m,
            altura = 1.70m,
            habitosAlimenticios = "Normal",
            antecedentesClinicos = "Ninguno",
            necesidadesEspecificas = "Ninguna"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/pacientes/{idInexistente}/consulta-inicial", requestConsulta);

        // Assert — el handler lanza KeyNotFoundException; el middleware lo mapea a 404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // -----------------------------------------------------------------------
    // Test 4 — Flujo incorrecto: consulta inicial duplicada (regla de dominio)
    // -----------------------------------------------------------------------
    [Fact]
    public async Task RegistrarConsultaInicial_DosVeces_Retorna400()
    {
        // Arrange: crear paciente y registrar primera consulta
        var pacienteId = await CrearPacienteAsync("Elena", "Vargas");

        var requestConsulta = new
        {
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            peso = 62.0m,
            altura = 1.65m,
            habitosAlimenticios = "Vegetariana",
            antecedentesClinicos = "Ninguno",
            necesidadesEspecificas = "Sin gluten"
        };

        // Primera consulta — debe ser exitosa
        var primeraRespuesta = await _client.PostAsJsonAsync($"/api/pacientes/{pacienteId}/consulta-inicial", requestConsulta);
        Assert.Equal(HttpStatusCode.NoContent, primeraRespuesta.StatusCode);

        // Act — segunda consulta al mismo paciente
        var segundaRespuesta = await _client.PostAsJsonAsync($"/api/pacientes/{pacienteId}/consulta-inicial", requestConsulta);

        // Assert — el dominio lanza InvalidOperationException (regla de negocio);
        // el middleware lo mapea a 400
        Assert.Equal(HttpStatusCode.BadRequest, segundaRespuesta.StatusCode);
    }
}

// DTO para deserializar la respuesta de creación
file record CreatedIdDto(Guid Id);
