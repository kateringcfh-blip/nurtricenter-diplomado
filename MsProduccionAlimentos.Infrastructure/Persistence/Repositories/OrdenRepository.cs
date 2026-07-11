using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Infrastructure.Persistence.Repositories;

public class OrdenRepository : IOrdenRepository
{
    private readonly NurTricenterDbContext _context;

    public OrdenRepository(NurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task Agregar(OrdenProduccion orden)
        => await _context.OrdenesProd.AddAsync(orden);

    public async Task<OrdenProduccion?> ObtenerPorId(OrdenId id)
        => await _context.OrdenesProd
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<OrdenProduccion>> ObtenerTodos()
        => await _context.OrdenesProd.ToListAsync();

    public async Task GuardarCambios()
        => await _context.SaveChangesAsync();
}
