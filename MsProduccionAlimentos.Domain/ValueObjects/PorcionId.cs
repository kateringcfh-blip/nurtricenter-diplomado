namespace MsProduccionAlimentos.Domain.ValueObjects;

public readonly record struct PorcionId
{
    public Guid Valor { get; }

    private PorcionId(Guid valor) => Valor = valor;

    public static PorcionId Crear() => new(Guid.NewGuid());

    public static PorcionId De(Guid valor)
    {
        if (valor == Guid.Empty)
            throw new ArgumentException("PorcionId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
