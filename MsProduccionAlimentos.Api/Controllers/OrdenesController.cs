using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsProduccionAlimentos.Application.Commands.CancelarOrden;
using MsProduccionAlimentos.Application.Commands.GenerarOrden;
using MsProduccionAlimentos.Application.Queries.ListarOrdenes;
using MsProduccionAlimentos.Application.Queries.ObtenerOrdenPorId;

namespace MsProduccionAlimentos.Api.Controllers;

[ApiController]
[Route("api/ordenes")]
public class OrdenesController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdenesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Generar([FromBody] GenerarOrdenCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var ordenes = await _mediator.Send(new ListarOrdenesQuery());
        return Ok(ordenes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var orden = await _mediator.Send(new ObtenerOrdenPorIdQuery(id));
        return orden is null ? NotFound() : Ok(orden);
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarOrdenRequest request)
    {
        await _mediator.Send(new CancelarOrdenCommand(id, request.Motivo));
        return NoContent();
    }
}

public record CancelarOrdenRequest(string Motivo);
