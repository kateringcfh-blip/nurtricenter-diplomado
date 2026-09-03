using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Domain.Entities;

public class ConsultaInicial
{
    public ConsultaId Id { get; private set; }
    public DateOnly Fecha { get; private set; }
    public decimal Peso { get; private set; }
    public decimal Altura { get; private set; }
    public Anamnesis Anamnesis { get; private set; }

    private ConsultaInicial() { Anamnesis = null!; }

    private ConsultaInicial(ConsultaId id, DateOnly fecha, decimal peso, decimal altura, Anamnesis anamnesis)
    {
        Id = id;
        Fecha = fecha;
        Peso = peso;
        Altura = altura;
        Anamnesis = anamnesis;
    }

    public static ConsultaInicial Registrar(DateOnly fecha, decimal peso, decimal altura, Anamnesis anamnesis)
    {
        if (peso <= 0) throw new ArgumentException("El peso debe ser mayor a cero.", nameof(peso));
        if (altura <= 0) throw new ArgumentException("La altura debe ser mayor a cero.", nameof(altura));
        ArgumentNullException.ThrowIfNull(anamnesis);

        return new(ConsultaId.Crear(), fecha, peso, altura, anamnesis);
    }

    public decimal CalcularImc() => Peso / (Altura * Altura);

    public override bool Equals(object? obj) => obj is ConsultaInicial other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
