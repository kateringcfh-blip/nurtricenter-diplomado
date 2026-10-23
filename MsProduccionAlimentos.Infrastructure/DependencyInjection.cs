using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Infrastructure.ExternalClients;
using MsProduccionAlimentos.Infrastructure.Messaging;
using MsProduccionAlimentos.Infrastructure.Outbox;
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

        services.AddHttpClient<IPacientesApiClient, PacientesApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["PacientesApi:BaseUrl"] ?? "http://localhost:5144");
        });

        // Mensajería (RabbitMQ): publica OrdenCancelada y consume PacienteRegistrado.
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddSingleton<IReceivedIntegrationEventsStore, InMemoryReceivedIntegrationEventsStore>();
        services.AddHostedService<PacienteRegistradoConsumerService>();

        // Patrón Transactional Outbox: el handler escribe en OutboxMessages dentro de la
        // misma transacción que el aggregate (EfOutboxWriter); este BackgroundService la
        // publica a RabbitMQ por separado, garantizando entrega "al menos una vez" aunque
        // el proceso se caiga justo después de guardar en la base.
        services.AddScoped<IIntegrationEventOutbox, EfOutboxWriter>();
        services.AddHostedService<OutboxPublisherBackgroundService>();

        return services;
    }
}
