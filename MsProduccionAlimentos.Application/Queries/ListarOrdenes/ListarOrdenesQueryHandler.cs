using MediatR;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Application.Queries.ListarOrdenes;

public class ListarOrdenesQueryHandler : IRequestHandler<ListarOrdenesQuery, List<OrdenDto>>
{
    private readonly IOrdenRepository _repository;

    public ListarOrdenesQueryHandler(IOrdenRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrdenDto>> Handle(ListarOrdenesQuery request, CancellationToken cancellationToken)
    {
        var ordenes = await _repository.ObtenerTodos();
        return ordenes.Select(MapearADto).ToList();
    }

    private static OrdenDto MapearADto(OrdenProduccion orden) => new(
        Id: orden.Id.Valor,
        Fecha: orden.Fecha,
        Estado: orden.Estado.ToString(),
        LoteProduccion: orden.LoteProduccion,
        TotalItems: orden.Items.Count,
        ItemsCompletos: orden.Items.Count(i => i.EstaCompleto()));
}
