using MediatR;
using MsPacientesEvaluacion.Application.DTOs;

namespace MsPacientesEvaluacion.Application.Queries.ObtenerPacientePorId;

public record ObtenerPacientePorIdQuery(Guid PacienteId) : IRequest<PacienteDetalleDto?>;
