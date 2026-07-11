using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsProduccionAlimentos.Application.Commands.ArmarPaquete;
using MsProduccionAlimentos.Application.Commands.EntregarALogistica;
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

    [HttpPost("{id:guid}/entregar")]
    public async Task<IActionResult> EntregarALogistica(Guid id)
    {
        await _mediator.Send(new EntregarALogisticaCommand(id));
        return NoContent();
    }
}
