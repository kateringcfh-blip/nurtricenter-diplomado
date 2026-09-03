using Microsoft.EntityFrameworkCore;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;
using MsPacientesEvaluacion.Infrastructure.Persistence;

namespace MsPacientesEvaluacion.Infrastructure.Persistence.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly PacientesDbContext _context;

    public PacienteRepository(PacientesDbContext context)
    {
        _context = context;
    }

    public async Task Agregar(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
    }

    public async Task<Paciente?> ObtenerPorId(PacienteId id)
    {
        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Paciente>> ObtenerTodos()
    {
        return await _context.Pacientes.ToListAsync();
    }
}
