using MediatR;
using MsPacientesEvaluacion.Application.DTOs;

namespace MsPacientesEvaluacion.Application.Queries.ListarPacientes;

public record ListarPacientesQuery : IRequest<List<PacienteDto>>;
