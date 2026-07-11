using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Infrastructure.Persistence.Repositories;

public class PaqueteRepository : IPaqueteRepository
{
    private readonly NurTricenterDbContext _context;

    public PaqueteRepository(NurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task Agregar(Paquete paquete)
        => await _context.Paquetes.AddAsync(paquete);

    public async Task<Paquete?> ObtenerPorId(PaqueteId id)
        => await _context.Paquetes
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task GuardarCambios()
        => await _context.SaveChangesAsync();
}
