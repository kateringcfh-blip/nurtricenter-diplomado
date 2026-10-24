using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsProduccionAlimentos.Application.Interfaces;

namespace MsProduccionAlimentos.Api.Controllers;

/// <summary>
/// Endpoint de apoyo para la demo/defensa: permite verificar, vía HTTP, los últimos
/// eventos de integración recibidos por RabbitMQ sin depender únicamente de los logs.
/// Se deja sin autenticación a propósito (solo lectura, sin datos sensibles) para no
/// complicar la demostración en vivo.
/// </summary>
[ApiController]
[Route("api/integracion")]
[AllowAnonymous]
public class IntegracionController : ControllerBase
{
    private readonly IReceivedIntegrationEventsStore _store;

    public IntegracionController(IReceivedIntegrationEventsStore store)
    {
        _store = store;
    }

    [HttpGet("eventos-recibidos")]
    public IActionResult ObtenerEventosRecibidos([FromQuery] int cantidad = 20)
    {
        var eventos = _store.ObtenerUltimos(cantidad);
        return Ok(eventos);
    }
}
