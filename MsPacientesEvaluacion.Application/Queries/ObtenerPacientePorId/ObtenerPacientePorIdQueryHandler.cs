using MediatR;
using Microsoft.EntityFrameworkCore;
using MsPacientesEvaluacion.Application.DTOs;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Queries.ObtenerPacientePorId;

public class ObtenerPacientePorIdQueryHandler : IRequestHandler<ObtenerPacientePorIdQuery, PacienteDetalleDto?>
{
    private readonly IPacientesDbContext _context;

    public ObtenerPacientePorIdQueryHandler(IPacientesDbContext context)
    {
        _context = context;
    }

    public async Task<PacienteDetalleDto?> Handle(ObtenerPacientePorIdQuery request, CancellationToken cancellationToken)
    {
        var pacienteId = PacienteId.De(request.PacienteId);

        return await _context.Pacientes
            .Where(p => p.Id == pacienteId)
            .Select(p => new PacienteDetalleDto(
                p.Id.Valor,
                p.Nombre,
                p.Apellido,
                p.FechaNacimiento,
                p.Estado.ToString(),
                p.Contacto.Email,
                p.Contacto.Telefono,
                p.ConsultaInicial != null,
                p.Evaluaciones.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
