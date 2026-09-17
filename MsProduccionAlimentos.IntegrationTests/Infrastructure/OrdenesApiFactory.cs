// Se usa SQLite embebido porque LocalDB, Docker/Testcontainers y SQL Server Express no están
// disponibles/funcionales en esta máquina; SQLite sigue ejecutando SQL real contra una base
// de datos real, cumpliendo el objetivo de integration testing sin depender de infraestructura externa.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsProduccionAlimentos.Infrastructure.Persistence;
using Xunit;

namespace MsProduccionAlimentos.IntegrationTests.Infrastructure;

public class OrdenesApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"nurtricenter_test_ordenes_{Guid.NewGuid():N}.db");

    private string ConnectionString => $"Data Source={_dbPath}";

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<NurTricenterDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<NurTricenterDbContext>(options =>
                options.UseSqlite(ConnectionString));
        });

        builder.UseEnvironment("Test");
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NurTricenterDbContext>();
        await context.Database.EnsureDeletedAsync();

        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }
}
