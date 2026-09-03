using MediatR;
using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;

namespace MsProduccionAlimentos.Application.Queries.ListarOrdenes;

public class ListarOrdenesQueryHandler : IRequestHandler<ListarOrdenesQuery, List<OrdenDto>>
{
    private readonly INurTricenterDbContext _context;

    public ListarOrdenesQueryHandler(INurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrdenDto>> Handle(ListarOrdenesQuery request, CancellationToken cancellationToken)
    {
        return await _context.OrdenesProd
            .Select(o => new OrdenDto(
                o.Id.Valor,
                o.Fecha,
                o.Estado.ToString(),
                o.LoteProduccion,
                o.Items.Count,
                o.Items.Count(i => i.CantidadPreparada >= i.CantidadRequerida)))
            .ToListAsync(cancellationToken);
    }
}
