namespace MsProduccionAlimentos.Domain.ValueObjects;

public readonly record struct PaqueteId
{
    public Guid Valor { get; }

    private PaqueteId(Guid valor) => Valor = valor;

    public static PaqueteId Crear() => new(Guid.NewGuid());

    public static PaqueteId De(Guid valor)
    {
        if (valor == Guid.Empty)
            throw new ArgumentException("PaqueteId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
