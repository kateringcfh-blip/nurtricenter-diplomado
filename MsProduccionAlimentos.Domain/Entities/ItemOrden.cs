using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Domain.Entities;

public class ItemOrden
{
    public ItemOrdenId Id { get; }
    public Guid RecetaId { get; }
    public int CantidadRequerida { get; }
    public int CantidadPreparada { get; private set; }

    private ItemOrden(ItemOrdenId id, Guid recetaId, int cantidadRequerida)
    {
        Id = id;
        RecetaId = recetaId;
        CantidadRequerida = cantidadRequerida;
        CantidadPreparada = 0;
    }

    public static ItemOrden Crear(Guid recetaId, int cantidadRequerida)
    {
        if (recetaId == Guid.Empty)
            throw new ArgumentException("RecetaId no puede ser un Guid vacío.", nameof(recetaId));
        if (cantidadRequerida <= 0)
            throw new ArgumentException("La cantidad requerida debe ser mayor a cero.", nameof(cantidadRequerida));

        return new(ItemOrdenId.Crear(), recetaId, cantidadRequerida);
    }

    public void MarcarPreparado(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a marcar debe ser mayor a cero.", nameof(cantidad));
        if (CantidadPreparada + cantidad > CantidadRequerida)
            throw new InvalidOperationException(
                $"No se puede preparar {cantidad} unidad(es) más: excedería la cantidad requerida ({CantidadRequerida}).");

        CantidadPreparada += cantidad;
    }

    public bool EstaCompleto() => CantidadPreparada >= CantidadRequerida;

    public override bool Equals(object? obj) => obj is ItemOrden other && Id.Equals(other.Id);
    public override int GetHashCode() => Id.GetHashCode();
}
