using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Domain.Entities;

public class Anamnesis
{
    public AnamnesisId Id { get; private set; }
    public string HabitosAlimenticios { get; private set; }
    public string AntecedentesClinicos { get; private set; }
    public string NecesidadesEspecificas { get; private set; }

    private Anamnesis()
    {
        HabitosAlimenticios = null!;
        AntecedentesClinicos = null!;
        NecesidadesEspecificas = null!;
    }

    private Anamnesis(AnamnesisId id, string habitosAlimenticios, string antecedentesClinicos, string necesidadesEspecificas)
    {
        Id = id;
        HabitosAlimenticios = habitosAlimenticios;
        AntecedentesClinicos = antecedentesClinicos;
        NecesidadesEspecificas = necesidadesEspecificas;
    }

    public static Anamnesis Registrar(string habitosAlimenticios, string antecedentesClinicos, string necesidadesEspecificas)
    {
        if (string.IsNullOrWhiteSpace(habitosAlimenticios))
            throw new ArgumentException("Los hábitos alimenticios no pueden estar vacíos.", nameof(habitosAlimenticios));
        if (string.IsNullOrWhiteSpace(antecedentesClinicos))
            throw new ArgumentException("Los antecedentes clínicos no pueden estar vacíos.", nameof(antecedentesClinicos));

        return new(AnamnesisId.Crear(), habitosAlimenticios, antecedentesClinicos, necesidadesEspecificas ?? string.Empty);
    }

    public bool TieneRestricciones() => !string.IsNullOrWhiteSpace(NecesidadesEspecificas);

    public override bool Equals(object? obj) => obj is Anamnesis other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
