namespace MsPacientesEvaluacion.Domain.ValueObjects;

public readonly record struct PacienteId
{
    public Guid Valor { get; }
    private PacienteId(Guid valor) => Valor = valor;
    public static PacienteId Crear() => new(Guid.NewGuid());
    public static PacienteId De(Guid valor)
    {
        if (valor == Guid.Empty) throw new ArgumentException("PacienteId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
