using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsPacientesEvaluacion.Application.Commands.AsignarNutricionista;
using MsPacientesEvaluacion.Application.Commands.DesactivarPaciente;
using MsPacientesEvaluacion.Application.Commands.RegistrarConsultaInicial;
using MsPacientesEvaluacion.Application.Commands.RegistrarEvaluacion;
using MsPacientesEvaluacion.Application.Commands.RegistrarPaciente;
using MsPacientesEvaluacion.Application.Queries.ListarPacientes;
using MsPacientesEvaluacion.Application.Queries.ObtenerPacientePorId;

namespace MsPacientesEvaluacion.Api.Controllers;

[ApiController]
[Route("api/pacientes")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarPaciente([FromBody] RegistrarPacienteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListarPacientes()
    {
        var pacientes = await _mediator.Send(new ListarPacientesQuery());
        return Ok(pacientes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var paciente = await _mediator.Send(new ObtenerPacientePorIdQuery(id));
        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost("{id:guid}/consulta-inicial")]
    public async Task<IActionResult> RegistrarConsultaInicial(Guid id, [FromBody] RegistrarConsultaInicialRequest request)
    {
        var command = new RegistrarConsultaInicialCommand(
            id,
            request.Fecha,
            request.Peso,
            request.Altura,
            request.HabitosAlimenticios,
            request.AntecedentesClinicos,
            request.NecesidadesEspecificas);

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/evaluaciones")]
    public async Task<IActionResult> RegistrarEvaluacion(Guid id, [FromBody] RegistrarEvaluacionRequest request)
    {
        var command = new RegistrarEvaluacionCommand(
            id,
            request.Fecha,
            request.Peso,
            request.Cintura,
            request.Cadera,
            request.Imc,
            request.Observaciones,
            request.Adherencia,
            request.PlanId);

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/asignar-nutricionista")]
    public async Task<IActionResult> AsignarNutricionista(Guid id, [FromBody] AsignarNutricionistaRequest request)
    {
        await _mediator.Send(new AsignarNutricionistaCommand(id, request.NutricionistaId));
        return NoContent();
    }

    [HttpPost("{id:guid}/desactivar")]
    public async Task<IActionResult> DesactivarPaciente(Guid id)
    {
        await _mediator.Send(new DesactivarPacienteCommand(id));
        return NoContent();
    }
}
