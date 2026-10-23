using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Infrastructure.Messaging;
using MsPacientesEvaluacion.Infrastructure.Persistence;
using MsPacientesEvaluacion.Infrastructure.Persistence.Repositories;

namespace MsPacientesEvaluacion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PacientesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PacientesConnection")));

        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPacientesDbContext>(sp => sp.GetRequiredService<PacientesDbContext>());

        // Mensajería (RabbitMQ): publica PacienteRegistrado y consume OrdenCancelada.
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddSingleton<IReceivedIntegrationEventsStore, InMemoryReceivedIntegrationEventsStore>();
        services.AddHostedService<OrdenCanceladaConsumerService>();

        return services;
    }
}
