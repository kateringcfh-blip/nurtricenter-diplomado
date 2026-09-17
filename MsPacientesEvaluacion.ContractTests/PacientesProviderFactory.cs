using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsPacientesEvaluacion.Infrastructure.Persistence;

namespace MsPacientesEvaluacion.ContractTests;

public class PacientesProviderFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"nurtricenter_pact_{Guid.NewGuid():N}.db");

    // Pooling=False evita que SQLite mantenga el handle abierto al hacer Dispose
    private string ConnectionString => $"Data Source={_dbPath};Pooling=False";

    // Host real con Kestrel — usado para todo acceso a Services y ServerUri
    // No usamos this.Services ni this.Server (heredados de WebApplicationFactory)
    // porque tienen un cast forzado a TestServer que falla con Kestrel real
    private IHost? _realHost;

    // URL real de Kestrel — asignada por el SO con puerto libre
    public Uri ServerUri { get; private set; } = null!;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHost =>
            webHost.UseKestrel().UseUrls("http://127.0.0.1:0"));

        _realHost = base.CreateHost(builder);
        _realHost.Start();

        var server = _realHost.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()!;
        ServerUri = new Uri(addresses.Addresses.First());

        return _realHost;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<PacientesDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<PacientesDbContext>(options =>
                options.UseSqlite(ConnectionString));

            services.AddControllers()
                .AddApplicationPart(typeof(ProviderStateController).Assembly);
        });

        builder.UseEnvironment("Test");
    }

    public void InitializeDatabase()
    {
        EnsureHostStarted();
        using var scope = _realHost!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PacientesDbContext>();
        context.Database.EnsureCreated();
    }

    // Dispara la construcción lazy del host accediendo a this.Server.
    // El InvalidCastException (KestrelServerImpl → TestServer) es esperado y se descarta:
    // ocurre DESPUÉS de que CreateHost ya guardó _realHost — solo el cast interno de la
    // clase base falla, nuestra lógica ya corrió correctamente.
    private void EnsureHostStarted()
    {
        if (_realHost is not null) return;
        try { _ = Server; } catch (InvalidCastException) { }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        SqliteConnection.ClearAllPools();

        if (File.Exists(_dbPath)) File.Delete(_dbPath);
    }
}
