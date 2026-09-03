using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsProduccionAlimentos.Application.Commands.CancelarOrden;
using MsProduccionAlimentos.Application.Commands.GenerarOrden;
using MsProduccionAlimentos.Application.Commands.GenerarOrdenDesdePedidos;
using MsProduccionAlimentos.Application.Commands.MarcarItemPreparado;
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
    
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var ordenes = await _mediator.Send(new ListarOrdenesQuery());
        return Ok(ordenes);
    }

    [HttpPost]
    public async Task<IActionResult> Generar([FromBody] GenerarOrdenCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }   

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var orden = await _mediator.Send(new ObtenerOrdenPorIdQuery(id));
        return orden is null ? NotFound() : Ok(orden);
    }

    [HttpPost("{id:guid}/items/{itemId:guid}/preparar")]
    public async Task<IActionResult> MarcarItemPreparado(Guid id, Guid itemId, [FromBody] MarcarItemPreparadoRequest request)
    {
        await _mediator.Send(new MarcarItemPreparadoCommand(id, itemId, request.Cantidad));
        return NoContent();
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarOrdenRequest request)
    {
        await _mediator.Send(new CancelarOrdenCommand(id, request.Motivo));
        return NoContent();
    }

    [HttpPost("desde-pedidos")]
    public async Task<IActionResult> GenerarDesdePedidos([FromBody] GenerarOrdenDesdePedidosCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = result.OrdenId }, result);
    }

}
public record MarcarItemPreparadoRequest(int Cantidad);
public record CancelarOrdenRequest(string Motivo);
