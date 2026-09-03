using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Application.Interfaces;

public interface INurTricenterDbContext
{
    IQueryable<OrdenProduccion> OrdenesProd { get; }
    IQueryable<Paquete> Paquetes { get; }
}
