using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace MsPacientesEvaluacion.ContractTests;

public class PacientesProviderContractTests : IClassFixture<PacientesProviderFactory>
{
    private readonly PacientesProviderFactory _factory;
    private readonly ITestOutputHelper _output;

    public PacientesProviderContractTests(PacientesProviderFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;

        // Inicializar la base de datos SQLite con el esquema
        _factory.InitializeDatabase();
    }

    [Fact]
    public void VerificarContratoConMsProduccionAlimentos()
    {
        var pactPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "pacts",
            "MsProduccionAlimentos-MsPacientesEvaluacion.json");

        // ServerUri es la URL real de Kestrel (ej. http://127.0.0.1:54321)
        // pact_ffi es un proceso nativo externo — necesita un socket real, no el TestServer en memoria
        var serverUri = _factory.ServerUri;

        using var verifier = new PactVerifier("MsPacientesEvaluacion", new PactVerifierConfig
        {
            Outputters = new List<IOutput> { new XunitOutputAdapter(_output) }
        });

        verifier
            .WithHttpEndpoint(serverUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(serverUri, "/provider-states"))
            .Verify();
    }

    // PactNet 5.x no incluye XunitOutput built-in — implementación inline de IOutput
    private sealed class XunitOutputAdapter : IOutput
    {
        private readonly ITestOutputHelper _xunit;
        public XunitOutputAdapter(ITestOutputHelper xunit) => _xunit = xunit;
        public void WriteLine(string line) => _xunit.WriteLine(line);
    }
}
