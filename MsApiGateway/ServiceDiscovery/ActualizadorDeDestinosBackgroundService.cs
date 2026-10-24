using Consul;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;
using ClusterConfig = Yarp.ReverseProxy.Configuration.ClusterConfig;
using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;
using InMemoryConfigProvider = Yarp.ReverseProxy.Configuration.InMemoryConfigProvider;

namespace MsApiGateway.ServiceDiscovery;

/// <summary>
/// Consulta a Consul periódicamente (Service Discovery) para saber qué instancias de
/// "pacientes" y "produccion" están saludables en este momento, y actualiza en caliente
/// los destinos de cada cluster de YARP mediante InMemoryConfigProvider.Update(). Si un
/// servicio se cae, su health check en Consul empieza a fallar, deja de aparecer en la
/// consulta y el Gateway deja de enviarle tráfico — sin reiniciar nada ni tocar
/// configuración estática.
/// </summary>
public sealed class ActualizadorDeDestinosBackgroundService : BackgroundService
{
    private static readonly string[] ServiciosConocidos = { "pacientes", "produccion" };

    private readonly IConsulClient _consulClient;
    private readonly InMemoryConfigProvider _configProvider;
    private readonly IReadOnlyList<RouteConfig> _rutas;
    private readonly TimeSpan _intervalo;
    private readonly ILogger<ActualizadorDeDestinosBackgroundService> _logger;

    public ActualizadorDeDestinosBackgroundService(
        IConsulClient consulClient,
        InMemoryConfigProvider configProvider,
        IReadOnlyList<RouteConfig> rutas,
        IConfiguration configuration,
        ILogger<ActualizadorDeDestinosBackgroundService> logger)
    {
        _consulClient = consulClient;
        _configProvider = configProvider;
        _rutas = rutas;
        _intervalo = TimeSpan.FromSeconds(configuration.GetValue("ServiceDiscovery:IntervaloSegundos", 30));
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Primera actualización inmediata: no tiene sentido esperar el intervalo completo
        // antes de poder enrutar tráfico la primera vez que arranca el Gateway.
        await ActualizarDestinosAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_intervalo, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            await ActualizarDestinosAsync(stoppingToken);
        }
    }

    private async Task ActualizarDestinosAsync(CancellationToken cancellationToken)
    {
        var nuevosClusters = new List<ClusterConfig>();

        foreach (var servicio in ServiciosConocidos)
        {
            var destinos = new Dictionary<string, DestinationConfig>();

            try
            {
                var respuesta = await _consulClient.Health.Service(servicio, null, true, cancellationToken);

                var indice = 0;
                foreach (var entrada in respuesta.Response)
                {
                    var direccion = entrada.Service.Address;
                    var puerto = entrada.Service.Port;
                    destinos[$"destino{indice}"] = new DestinationConfig { Address = $"{direccion}:{puerto}" };
                    indice++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[ServiceDiscovery] No se pudo consultar Consul para el servicio '{Servicio}'.", servicio);
            }

            nuevosClusters.Add(new ClusterConfig { ClusterId = $"{servicio}-cluster", Destinations = destinos });
            _logger.LogInformation("[ServiceDiscovery] {Servicio}: {Cantidad} instancia(s) saludable(s).", servicio, destinos.Count);
        }

        _configProvider.Update(_rutas, nuevosClusters);
    }
}
