using MediatR;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Queries.ObtenerOrdenPorId;

public class ObtenerOrdenPorIdQueryHandler : IRequestHandler<ObtenerOrdenPorIdQuery, OrdenDto?>
{
    private readonly IOrdenRepository _repository;

    public ObtenerOrdenPorIdQueryHandler(IOrdenRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrdenDto?> Handle(ObtenerOrdenPorIdQuery request, CancellationToken cancellationToken)
    {
        var orden = await _repository.ObtenerPorId(OrdenId.De(request.OrdenId));
        return orden is null ? null : MapearADto(orden);
    }

    private static OrdenDto MapearADto(OrdenProduccion orden) => new(
        Id: orden.Id.Valor,
        Fecha: orden.Fecha,
        Estado: orden.Estado.ToString(),
        LoteProduccion: orden.LoteProduccion,
        TotalItems: orden.Items.Count,
        ItemsCompletos: orden.Items.Count(i => i.EstaCompleto()));
}
