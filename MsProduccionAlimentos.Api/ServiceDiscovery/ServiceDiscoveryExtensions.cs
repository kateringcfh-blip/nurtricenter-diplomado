using Consul;

namespace MsProduccionAlimentos.Api.ServiceDiscovery;

public static class ServiceDiscoveryExtensions
{
    public static IServiceCollection RegisterServiceToServiceDiscovery(
        this IServiceCollection services, IConfiguration configuration)
    {
        var serviceDiscoveryAddress = configuration.GetValue<string>("ServiceDiscovery:ServiceDiscoveryAddress");

        services.AddSingleton<IConsulClient>(_ => new ConsulClient(config =>
        {
            if (!string.IsNullOrWhiteSpace(serviceDiscoveryAddress))
                config.Address = new Uri(serviceDiscoveryAddress);
        }));

        services.AddHostedService<ServiceRegistration>();

        return services;
    }
}
