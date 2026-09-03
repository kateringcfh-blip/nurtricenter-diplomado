using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Interfaces;

public interface IPaqueteRepository
{
    Task Agregar(Paquete paquete);
    Task<Paquete?> ObtenerPorId(PaqueteId id);
}
