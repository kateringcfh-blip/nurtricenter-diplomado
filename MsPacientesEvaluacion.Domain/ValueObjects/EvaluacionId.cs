namespace MsPacientesEvaluacion.Domain.ValueObjects;

public readonly record struct EvaluacionId
{
    public Guid Valor { get; }
    private EvaluacionId(Guid valor) => Valor = valor;
    public static EvaluacionId Crear() => new(Guid.NewGuid());
    public static EvaluacionId De(Guid valor)
    {
        if (valor == Guid.Empty) throw new ArgumentException("EvaluacionId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
