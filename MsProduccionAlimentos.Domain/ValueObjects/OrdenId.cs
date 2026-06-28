namespace MsProduccionAlimentos.Domain.ValueObjects;

public readonly record struct OrdenId
{
    public Guid Valor { get; }

    private OrdenId(Guid valor) => Valor = valor;

    public static OrdenId Crear() => new(Guid.NewGuid());

    public static OrdenId De(Guid valor)
    {
        if (valor == Guid.Empty)
            throw new ArgumentException("OrdenId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
