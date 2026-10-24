using Consul;

namespace MsProduccionAlimentos.Api.ServiceDiscovery;

/// <summary>
/// Registra esta instancia en Consul (Service Discovery) al arrancar y la da de baja al
/// apagarse (IHostedService.StartAsync/StopAsync). Consul, además, comprueba periódicamente
/// GET /health para saber si la instancia sigue respondiendo; si deja de hacerlo, el Gateway
/// deja de enrutarle tráfico automáticamente, sin tocar ninguna configuración estática.
/// </summary>
public sealed class ServiceRegistration : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceRegistration> _logger;
    private string? _registrationId;

    public ServiceRegistration(IConsulClient consulClient, IConfiguration configuration, ILogger<ServiceRegistration> logger)
    {
        _consulClient = consulClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string? serviceAddress = _configuration.GetValue<string>("ServiceDiscovery:Host");
        int? servicePort = _configuration.GetValue<int?>("ServiceDiscovery:Port");
        string? serviceName = _configuration.GetValue<string>("ServiceDiscovery:ServiceId");

        if (string.IsNullOrWhiteSpace(serviceAddress) || servicePort is null || string.IsNullOrWhiteSpace(serviceName))
        {
            _logger.LogWarning("[ServiceDiscovery] Falta configuración ServiceDiscovery:Host/Port/ServiceId; no se registra en Consul.");
            return;
        }

        // Consul corre dentro de un contenedor Docker; "localhost" ahí sería el propio
        // contenedor, no la máquina host donde realmente corre este servicio (dotnet run).
        // host.docker.internal es cómo Docker Desktop deja que un contenedor alcance al host.
        string healthCheckAddress = serviceAddress.Replace("localhost", "host.docker.internal");

        var registration = new AgentServiceRegistration
        {
            ID = $"{serviceName}-{Guid.NewGuid()}",
            Name = serviceName,
            Address = serviceAddress,
            Port = servicePort.Value,
            Check = new AgentServiceCheck
            {
                HTTP = $"{healthCheckAddress}:{servicePort}/health",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                Method = "GET",
            },
        };

        _registrationId = registration.ID;

        try
        {
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
            _logger.LogInformation(
                "[ServiceDiscovery] Registrado en Consul como '{ServiceId}' ({Address}:{Port}).",
                serviceName, serviceAddress, servicePort);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[ServiceDiscovery] No se pudo registrar en Consul (¿está levantado el contenedor?).");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_registrationId))
            return;

        try
        {
            await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[ServiceDiscovery] No se pudo dar de baja del registro en Consul durante el apagado.");
        }
    }
}
