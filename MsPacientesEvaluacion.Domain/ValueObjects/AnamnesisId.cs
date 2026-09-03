namespace MsPacientesEvaluacion.Domain.ValueObjects;

public readonly record struct AnamnesisId
{
    public Guid Valor { get; }
    private AnamnesisId(Guid valor) => Valor = valor;
    public static AnamnesisId Crear() => new(Guid.NewGuid());
    public static AnamnesisId De(Guid valor)
    {
        if (valor == Guid.Empty) throw new ArgumentException("AnamnesisId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
