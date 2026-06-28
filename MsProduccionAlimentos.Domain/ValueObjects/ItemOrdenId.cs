namespace MsProduccionAlimentos.Domain.ValueObjects;

public readonly record struct ItemOrdenId
{
    public Guid Valor { get; }

    private ItemOrdenId(Guid valor) => Valor = valor;

    public static ItemOrdenId Crear() => new(Guid.NewGuid());

    public static ItemOrdenId De(Guid valor)
    {
        if (valor == Guid.Empty)
            throw new ArgumentException("ItemOrdenId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
