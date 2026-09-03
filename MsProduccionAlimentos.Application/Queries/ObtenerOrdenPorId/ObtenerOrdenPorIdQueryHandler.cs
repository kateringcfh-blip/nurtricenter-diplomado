using MediatR;
using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Queries.ObtenerOrdenPorId;

public class ObtenerOrdenPorIdQueryHandler : IRequestHandler<ObtenerOrdenPorIdQuery, OrdenDto?>
{
    private readonly INurTricenterDbContext _context;

    public ObtenerOrdenPorIdQueryHandler(INurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task<OrdenDto?> Handle(ObtenerOrdenPorIdQuery request, CancellationToken cancellationToken)
    {
        var ordenId = OrdenId.De(request.OrdenId);

        return await _context.OrdenesProd
            .Where(o => o.Id == ordenId)
            .Select(o => new OrdenDto(
                o.Id.Valor,
                o.Fecha,
                o.Estado.ToString(),
                o.LoteProduccion,
                o.Items.Count,
                o.Items.Count(i => i.CantidadPreparada >= i.CantidadRequerida)))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
