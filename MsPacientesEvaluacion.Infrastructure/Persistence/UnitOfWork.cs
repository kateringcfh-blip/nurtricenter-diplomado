using MsPacientesEvaluacion.Application.Interfaces;

namespace MsPacientesEvaluacion.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PacientesDbContext _context;

    public UnitOfWork(PacientesDbContext context)
    {
        _context = context;
    }

    public async Task<int> GuardarCambios(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
