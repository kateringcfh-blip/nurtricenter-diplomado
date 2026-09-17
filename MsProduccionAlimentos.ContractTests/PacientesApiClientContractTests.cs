using System.Net;
using PactNet;
using PactNet.Matchers;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Infrastructure.ExternalClients;
using Xunit;
using Xunit.Abstractions;

namespace MsProduccionAlimentos.ContractTests;

public class PacientesApiClientContractTests
{
    // Pact.V3(...).WithHttpInteractions() retorna IPactBuilderV3 — ahí viven UponReceiving/VerifyAsync
    private readonly IPactBuilderV3 _pact;

    public PacientesApiClientContractTests(ITestOutputHelper output)
    {
        var pactsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "pacts");
        Directory.CreateDirectory(pactsDir);

        var config = new PactConfig
        {
            PactDir = pactsDir,
            LogLevel = PactLogLevel.Information
        };

        _pact = Pact.V3("MsProduccionAlimentos", "MsPacientesEvaluacion", config)
                    .WithHttpInteractions();
    }

    // Interaccion 1 — paciente existente → 200 con datos completos
    [Fact]
    public async Task ObtenerPaciente_PacienteExistente_Retorna200ConDatosCompletos()
    {
        var pacienteId = new Guid("11111111-1111-1111-1111-111111111111");

        _pact
            .UponReceiving("una solicitud de paciente existente con id 11111111-1111-1111-1111-111111111111")
            .Given("un paciente con id 11111111-1111-1111-1111-111111111111 existe")
            .WithRequest(HttpMethod.Get, $"/api/pacientes/{pacienteId}")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", Match.Type("application/json; charset=utf-8"))
            .WithJsonBody(new
            {
                id = Match.Type(pacienteId),
                nombre = Match.Type("Carlos"),
                apellido = Match.Type("Perez"),
                fechaNacimiento = Match.Type("1985-03-15"),
                estado = Match.Type("Activo"),
                email = Match.Type("carlos.perez@mail.com"),
                telefono = Match.Type("555-0001"),
                tieneConsultaInicial = Match.Type(false),
                totalEvaluaciones = Match.Type(0)
            });

        await _pact.VerifyAsync(async ctx =>
        {
            var client = new PacientesApiClient(new HttpClient { BaseAddress = ctx.MockServerUri });
            var resultado = await client.ObtenerPaciente(pacienteId);

            Assert.NotNull(resultado);
            Assert.Equal(pacienteId, resultado.Id);
            Assert.NotNull(resultado.Nombre);
            Assert.NotNull(resultado.Apellido);
            Assert.NotNull(resultado.Estado);
            Assert.NotNull(resultado.Email);
        });
    }

    // Interaccion 2 — paciente inexistente → 404, cliente retorna null
    [Fact]
    public async Task ObtenerPaciente_PacienteInexistente_RetornaNull()
    {
        var pacienteId = new Guid("99999999-9999-9999-9999-999999999999");

        _pact
            .UponReceiving("una solicitud de paciente inexistente con id 99999999-9999-9999-9999-999999999999")
            .Given("el paciente 99999999-9999-9999-9999-999999999999 no existe")
            .WithRequest(HttpMethod.Get, $"/api/pacientes/{pacienteId}")
            .WillRespond()
            .WithStatus(HttpStatusCode.NotFound);

        await _pact.VerifyAsync(async ctx =>
        {
            var client = new PacientesApiClient(new HttpClient { BaseAddress = ctx.MockServerUri });
            var resultado = await client.ObtenerPaciente(pacienteId);

            Assert.Null(resultado);
        });
    }
}
