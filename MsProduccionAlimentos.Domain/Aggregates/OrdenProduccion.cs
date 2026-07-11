using MsProduccionAlimentos.Domain.Entities;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Domain.Events;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Domain.Aggregates;

public class OrdenProduccion
{
    private readonly List<ItemOrden> _items = new();
    private readonly List<IDomainEvent> _eventos = new();

    public OrdenId Id { get; }
    public DateOnly Fecha { get; }
    public EstadoOrden Estado { get; private set; }
    public Guid EncargadoCocinaId { get; }
    public string LoteProduccion { get; }
    public IReadOnlyList<ItemOrden> Items => _items.AsReadOnly();
    public IReadOnlyList<IDomainEvent> EventosOcurridos => _eventos.AsReadOnly();

    private OrdenProduccion() { LoteProduccion = null!; }  // EF Core

    private OrdenProduccion(OrdenId id, DateOnly fecha, Guid encargadoCocinaId, string loteProduccion)
    {
        Id = id;
        Fecha = fecha;
        Estado = EstadoOrden.Pendiente;
        EncargadoCocinaId = encargadoCocinaId;
        LoteProduccion = loteProduccion;
    }

    public static OrdenProduccion Generar(Guid encargadoCocinaId, string loteProduccion, DateOnly fecha)
    {
        if (encargadoCocinaId == Guid.Empty)
            throw new ArgumentException("El encargado de cocina no puede ser un Guid vacío.", nameof(encargadoCocinaId));
        if (string.IsNullOrWhiteSpace(loteProduccion))
            throw new ArgumentException("El lote de producción no puede estar vacío.", nameof(loteProduccion));

        return new(OrdenId.Crear(), fecha, encargadoCocinaId, loteProduccion);
    }

    public void AgregarItem(Guid recetaId, int cantidadRequerida)
    {
        if (Estado != EstadoOrden.Pendiente)
            throw new InvalidOperationException(
                $"No se pueden agregar items a una orden en estado '{Estado}'.");

        _items.Add(ItemOrden.Crear(recetaId, cantidadRequerida));
    }

    public void MarcarItemPreparado(ItemOrdenId itemOrdenId, int cantidad)
    {
        var item = _items.FirstOrDefault(i => i.Id.Equals(itemOrdenId))
            ?? throw new InvalidOperationException(
                $"No se encontró el item con Id '{itemOrdenId.Valor}' en la orden.");

        item.MarcarPreparado(cantidad);
    }

    public void Completar()
    {
        var incompletos = _items.Where(i => !i.EstaCompleto()).ToList();
        if (incompletos.Count != 0)
        {
            var ids = string.Join(", ", incompletos.Select(i => i.Id.Valor));
            throw new InvalidOperationException(
                $"No se puede completar la orden: los siguientes items aún no están listos: {ids}.");
        }

        Estado = EstadoOrden.Completada;
    }

    public void Cancelar(string motivo)
    {
        if (Estado == EstadoOrden.Completada)
            throw new InvalidOperationException("No se puede cancelar una orden ya completada.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Se debe indicar un motivo para cancelar la orden.", nameof(motivo));

        Estado = EstadoOrden.Cancelada;
        _eventos.Add(OrdenCancelada.Crear(Id, motivo));
    }

    public void LimpiarEventos() => _eventos.Clear();
}
