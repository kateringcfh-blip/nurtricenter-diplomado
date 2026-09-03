using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Infrastructure.Persistence;
using MsProduccionAlimentos.Infrastructure.Persistence.Repositories;

namespace MsProduccionAlimentos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NurTricenterDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        services.AddScoped<IOrdenRepository, OrdenRepository>();
        services.AddScoped<IPaqueteRepository, PaqueteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INurTricenterDbContext>(sp =>
            sp.GetRequiredService<NurTricenterDbContext>());

        return services;
    }
}