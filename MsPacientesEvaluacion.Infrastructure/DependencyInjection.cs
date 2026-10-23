using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Infrastructure.Messaging;
using MsPacientesEvaluacion.Infrastructure.Outbox;
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

        // Patrón Transactional Outbox: el handler escribe en OutboxMessages dentro de la
        // misma transacción que el aggregate (EfOutboxWriter); este BackgroundService la
        // publica a RabbitMQ por separado, garantizando entrega "al menos una vez" aunque
        // el proceso se caiga justo después de guardar en la base.
        services.AddScoped<IIntegrationEventOutbox, EfOutboxWriter>();
        services.AddHostedService<OutboxPublisherBackgroundService>();

        return services;
    }
}
