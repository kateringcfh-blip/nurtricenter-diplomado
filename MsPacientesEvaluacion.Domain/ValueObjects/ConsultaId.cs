namespace MsPacientesEvaluacion.Domain.ValueObjects;

public readonly record struct ConsultaId
{
    public Guid Valor { get; }
    private ConsultaId(Guid valor) => Valor = valor;
    public static ConsultaId Crear() => new(Guid.NewGuid());
    public static ConsultaId De(Guid valor)
    {
        if (valor == Guid.Empty) throw new ArgumentException("ConsultaId no puede ser un Guid vacío.", nameof(valor));
        return new(valor);
    }
}
