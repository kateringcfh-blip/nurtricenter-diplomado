using Consul;
using MsApiGateway.ServiceDiscovery;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;
using ClusterConfig = Yarp.ReverseProxy.Configuration.ClusterConfig;
using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;
using RouteMatch = Yarp.ReverseProxy.Configuration.RouteMatch;
using InMemoryConfigProvider = Yarp.ReverseProxy.Configuration.InMemoryConfigProvider;
using IProxyConfigProvider = Yarp.ReverseProxy.Configuration.IProxyConfigProvider;

var builder = WebApplication.CreateBuilder(args);

var consulAddress = builder.Configuration.GetValue<string>("ServiceDiscovery:ConsulAddress") ?? "http://localhost:8500";
builder.Services.AddSingleton<IConsulClient>(_ => new ConsulClient(c => c.Address = new Uri(consulAddress)));

// --- Rutas y clusters de YARP --------------------------------------------------------
// Las RUTAS (qué prefijo de URL va a qué microservicio) son fijas y se conocen de
// antemano: /pacientes/** y /produccion/**. Los DESTINOS de cada cluster, en cambio,
// NO están escritos aquí: se resuelven en tiempo real contra Consul (Service Discovery)
// mediante el ActualizadorDeDestinosBackgroundService de abajo, que pregunta cada N
// segundos qué instancias están saludables y actualiza el proxy sin reiniciar nada.
var rutasFijas = new[]
{
    new RouteConfig
    {
        RouteId = "pacientes-route",
        ClusterId = "pacientes-cluster",
        Match = new RouteMatch { Path = "/pacientes/{**catch-all}" },
        Transforms = new[] { new Dictionary<string, string> { ["PathRemovePrefix"] = "/pacientes" } },
    },
    new RouteConfig
    {
        RouteId = "produccion-route",
        ClusterId = "produccion-cluster",
        Match = new RouteMatch { Path = "/produccion/{**catch-all}" },
        Transforms = new[] { new Dictionary<string, string> { ["PathRemovePrefix"] = "/produccion" } },
    },
};

var clustersIniciales = new[]
{
    new ClusterConfig { ClusterId = "pacientes-cluster", Destinations = new Dictionary<string, DestinationConfig>() },
    new ClusterConfig { ClusterId = "produccion-cluster", Destinations = new Dictionary<string, DestinationConfig>() },
};

var proveedorDeConfiguracion = new InMemoryConfigProvider(rutasFijas, clustersIniciales);
builder.Services.AddSingleton(proveedorDeConfiguracion);
builder.Services.AddSingleton<IProxyConfigProvider>(proveedorDeConfiguracion);
builder.Services.AddSingleton<IReadOnlyList<RouteConfig>>(rutasFijas);
builder.Services.AddReverseProxy();

builder.Services.AddHostedService<ActualizadorDeDestinosBackgroundService>();

var app = builder.Build();

// Endpoint informativo para confirmar rápidamente, en la demo, que el Gateway está arriba
// y cómo están repartidas las rutas entre los dos microservicios.
app.MapGet("/", () => Results.Ok(new
{
    servicio = "NurTricenter API Gateway",
    descripcion = "Enruta por prefijo hacia cada microservicio; los destinos se resuelven dinámicamente vía Consul (Service Discovery).",
    consulUi = "http://localhost:8500/ui",
    rutas = new[]
    {
        "/pacientes/{**ruta}   -> MsPacientesEvaluacion.Api (resuelto vía Consul)",
        "/produccion/{**ruta}  -> MsProduccionAlimentos.Api (resuelto vía Consul)",
    },
    ejemplos = new[]
    {
        "POST /pacientes/api/auth/login",
        "GET  /pacientes/api/pacientes",
        "POST /produccion/api/ordenes",
        "GET  /produccion/api/integracion/eventos-recibidos",
    }
}));

app.MapReverseProxy();

app.Run();
