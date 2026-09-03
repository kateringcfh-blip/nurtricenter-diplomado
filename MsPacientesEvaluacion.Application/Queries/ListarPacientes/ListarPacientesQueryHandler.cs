using MediatR;
using Microsoft.EntityFrameworkCore;
using MsPacientesEvaluacion.Application.DTOs;
using MsPacientesEvaluacion.Application.Interfaces;

namespace MsPacientesEvaluacion.Application.Queries.ListarPacientes;

public class ListarPacientesQueryHandler : IRequestHandler<ListarPacientesQuery, List<PacienteDto>>
{
    private readonly IPacientesDbContext _context;

    public ListarPacientesQueryHandler(IPacientesDbContext context)
    {
        _context = context;
    }

    public async Task<List<PacienteDto>> Handle(ListarPacientesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pacientes
            .Select(p => new PacienteDto(
                p.Id.Valor,
                p.Nombre,
                p.Apellido,
                p.Estado.ToString(),
                p.Evaluaciones.Count))
            .ToListAsync(cancellationToken);
    }
}
