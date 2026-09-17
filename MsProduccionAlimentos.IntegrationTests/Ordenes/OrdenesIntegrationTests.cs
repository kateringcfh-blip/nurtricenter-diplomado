using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Infrastructure.Persistence;
using MsProduccionAlimentos.IntegrationTests.Infrastructure;
using Xunit;

namespace MsProduccionAlimentos.IntegrationTests.Ordenes;

public class OrdenesIntegrationTests : IClassFixture<OrdenesApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly OrdenesApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public OrdenesIntegrationTests(OrdenesApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // -----------------------------------------------------------------------
    // Test 1 — Flujo correcto: GenerarOrdenDesdePedidos con dos pacientes
    // -----------------------------------------------------------------------
    [Fact]
    public async Task GenerarOrdenDesdePedidos_ConDosPacientes_Retorna201ConOrdenYDosPaquetes()
    {
        // Arrange
        var encargadoCocinaId = Guid.NewGuid();
        var receta1Id = Guid.NewGuid();
        var receta2Id = Guid.NewGuid();

        var request = new
        {
            encargadoCocinaId,
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            pedidos = new[]
            {
                new
                {
                    pacienteId = Guid.NewGuid(),
                    nombrePaciente = "Carlos Pérez",
                    direccionEntrega = "Av. Principal 123",
                    numeroId = "12345678",
                    recetas = new[]
                    {
                        new { recetaId = receta1Id, cantidad = 2 },
                        new { recetaId = receta2Id, cantidad = 1 }
                    }
                },
                new
                {
                    pacienteId = Guid.NewGuid(),
                    nombrePaciente = "Ana García",
                    direccionEntrega = "Calle Secundaria 456",
                    numeroId = "87654321",
                    recetas = new[]
                    {
                        new { recetaId = receta1Id, cantidad = 3 }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ordenes/desde-pedidos", request);

        // Assert — HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — Body
        var body = await response.Content.ReadFromJsonAsync<GenerarOrdenDesdePedidosResultDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.OrdenId);
        Assert.NotNull(body.PaqueteIds);
        Assert.Equal(2, body.PaqueteIds.Count);

        // Assert — Persistencia (ToList en memoria para evitar limitación de traducción de SQLite con Value Objects)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();

        var todasOrdenes = await context.OrdenesProd.ToListAsync();
        var orden = todasOrdenes.FirstOrDefault(o => o.Id.Valor == body.OrdenId);
        Assert.NotNull(orden);

        var todosPaquetes = await context.Paquetes.ToListAsync();
        var paquetes = todosPaquetes.Where(p => p.OrdenId.Valor == body.OrdenId).ToList();
        Assert.Equal(2, paquetes.Count);
    }

    // -----------------------------------------------------------------------
    // Test 2 — Flujo incorrecto: sin pedidos → 400
    // -----------------------------------------------------------------------
    [Fact]
    public async Task GenerarOrdenDesdePedidos_SinPedidos_Retorna400()
    {
        // Arrange
        var request = new
        {
            encargadoCocinaId = Guid.NewGuid(),
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            pedidos = Array.Empty<object>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ordenes/desde-pedidos", request);

        // Assert — el dominio lanza ArgumentException para lista vacía;
        // el middleware lo mapea a 400
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // -----------------------------------------------------------------------
    // Test 3 — Flujo correcto: POST /api/ordenes (endpoint simple)
    // -----------------------------------------------------------------------
    [Fact]
    public async Task GenerarOrdenPorEndpointSimple_ConDatosValidos_Retorna201YPersiste()
    {
        // Arrange
        var request = new
        {
            encargadoCocinaId = Guid.NewGuid(),
            loteProduccion = "LOTE-2026-001",
            fecha = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            items = new[]
            {
                new { recetaId = Guid.NewGuid(), cantidadRequerida = 5 },
                new { recetaId = Guid.NewGuid(), cantidadRequerida = 3 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ordenes", request);

        // Assert — HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — Body
        var body = await response.Content.ReadFromJsonAsync<CreatedIdDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);

        // Assert — Persistencia (ToList en memoria para evitar limitación de traducción de SQLite con Value Objects)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();

        var todasOrdenes = await context.OrdenesProd.ToListAsync();
        var orden = todasOrdenes.FirstOrDefault(o => o.Id.Valor == body.Id);
        Assert.NotNull(orden);
        // La orden recién creada debe estar en estado Pendiente
        Assert.Equal(EstadoOrden.Pendiente, orden.Estado);
    }
}

// DTOs de respuesta para deserializar
file record GenerarOrdenDesdePedidosResultDto(Guid OrdenId, List<Guid> PaqueteIds);
file record CreatedIdDto(Guid Id);
