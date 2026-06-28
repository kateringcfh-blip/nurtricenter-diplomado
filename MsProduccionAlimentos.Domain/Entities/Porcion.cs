using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Domain.Entities;

public class Porcion
{
    public PorcionId Id { get; }
    public Guid RecetaId { get; }
    public decimal Cantidad { get; }
    public bool EstaEnvasada { get; private set; }

    private Porcion(PorcionId id, Guid recetaId, decimal cantidad)
    {
        Id = id;
        RecetaId = recetaId;
        Cantidad = cantidad;
        EstaEnvasada = false;
    }

    public static Porcion Crear(Guid recetaId, decimal cantidad)
    {
        if (recetaId == Guid.Empty)
            throw new ArgumentException("RecetaId no puede ser un Guid vacío.", nameof(recetaId));
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

        return new(PorcionId.Crear(), recetaId, cantidad);
    }

    public void Envasar()
    {
        if (EstaEnvasada)
            throw new InvalidOperationException("La porción ya fue envasada.");

        EstaEnvasada = true;
    }

    public override bool Equals(object? obj) => obj is Porcion other && Id.Equals(other.Id);
    public override int GetHashCode() => Id.GetHashCode();
}
