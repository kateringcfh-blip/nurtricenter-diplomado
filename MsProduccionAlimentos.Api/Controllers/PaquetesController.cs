using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsProduccionAlimentos.Application.Commands.AgregarPorcion;
using MsProduccionAlimentos.Application.Commands.ArmarPaquete;
using MsProduccionAlimentos.Application.Commands.EntregarALogistica;
using MsProduccionAlimentos.Application.Commands.EnvasarPorcion;
using MsProduccionAlimentos.Application.Queries.ObtenerPaquetePorId;

namespace MsProduccionAlimentos.Api.Controllers;

[ApiController]
[Route("api/paquetes")]
public class PaquetesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaquetesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Armar([FromBody] ArmarPaqueteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var paquete = await _mediator.Send(new ObtenerPaquetePorIdQuery(id));
        return paquete is null ? NotFound() : Ok(paquete);
    }

    [HttpPost("{id:guid}/porciones")]
    public async Task<IActionResult> AgregarPorcion(Guid id, [FromBody] AgregarPorcionRequest request)
    {
        var porcionId = await _mediator.Send(new AgregarPorcionCommand(id, request.RecetaId, request.Cantidad));
        return Created($"/api/paquetes/{id}/porciones/{porcionId}", new { id = porcionId });
    }

    [HttpPost("{id:guid}/porciones/{porcionId:guid}/envasar")]
    public async Task<IActionResult> EnvasarPorcion(Guid id, Guid porcionId)
    {
        await _mediator.Send(new EnvasarPorcionCommand(id, porcionId));
        return NoContent();
    }

    [HttpPost("{id:guid}/entregar")]
    public async Task<IActionResult> EntregarALogistica(Guid id)
    {
        await _mediator.Send(new EntregarALogisticaCommand(id));
        return NoContent();
    }
}

public record AgregarPorcionRequest(Guid RecetaId, decimal Cantidad);
