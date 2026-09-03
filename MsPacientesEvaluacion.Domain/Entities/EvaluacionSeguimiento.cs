using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Domain.Entities;

public class EvaluacionSeguimiento
{
    public EvaluacionId Id { get; private set; }
    public DateOnly Fecha { get; private set; }
    public decimal Peso { get; private set; }
    public Medidas Medidas { get; private set; }
    public string Observaciones { get; private set; }
    public NivelAdherencia Adherencia { get; private set; }
    public Guid PlanId { get; private set; }

    private EvaluacionSeguimiento() { Medidas = null!; Observaciones = null!; }

    private EvaluacionSeguimiento(EvaluacionId id, DateOnly fecha, decimal peso, Medidas medidas,
        string observaciones, NivelAdherencia adherencia, Guid planId)
    {
        Id = id;
        Fecha = fecha;
        Peso = peso;
        Medidas = medidas;
        Observaciones = observaciones;
        Adherencia = adherencia;
        PlanId = planId;
    }

    public static EvaluacionSeguimiento Registrar(DateOnly fecha, decimal peso, Medidas medidas,
        string observaciones, NivelAdherencia adherencia, Guid planId)
    {
        if (peso <= 0) throw new ArgumentException("El peso debe ser mayor a cero.", nameof(peso));
        ArgumentNullException.ThrowIfNull(medidas);

        return new(EvaluacionId.Crear(), fecha, peso, medidas, observaciones ?? string.Empty, adherencia, planId);
    }

    /// <summary>
    /// Compara el peso actual con el de la evaluación anterior.
    /// Retorna "MEJORA" si bajó, "ESTABLE" si no cambió, "RETROCESO" si subió.
    /// </summary>
    public string CompararConAnterior(EvaluacionSeguimiento anterior)
    {
        ArgumentNullException.ThrowIfNull(anterior);
        return Peso < anterior.Peso ? "MEJORA"
             : Peso == anterior.Peso ? "ESTABLE"
             : "RETROCESO";
    }

    public override bool Equals(object? obj) => obj is EvaluacionSeguimiento other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
