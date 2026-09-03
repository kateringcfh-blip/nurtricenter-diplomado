using MsProduccionAlimentos.Application.Interfaces;

namespace MsProduccionAlimentos.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly NurTricenterDbContext _context;

    public UnitOfWork(NurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task<int> GuardarCambios(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
