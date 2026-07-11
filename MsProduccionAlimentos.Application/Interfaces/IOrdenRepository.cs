using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Interfaces;

public interface IOrdenRepository
{
    Task Agregar(OrdenProduccion orden);
    Task<OrdenProduccion?> ObtenerPorId(OrdenId id);
    Task<List<OrdenProduccion>> ObtenerTodos();
    Task GuardarCambios();
}
