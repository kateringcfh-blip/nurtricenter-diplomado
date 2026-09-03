namespace MsPacientesEvaluacion.Domain.ValueObjects;

public sealed record Medidas
{
    public decimal Cintura { get; }
    public decimal Cadera { get; }
    public decimal Imc { get; }

    private Medidas() { } // EF Core

    private Medidas(decimal cintura, decimal cadera, decimal imc)
    {
        Cintura = cintura;
        Cadera = cadera;
        Imc = imc;
    }

    public static Medidas Crear(decimal cintura, decimal cadera, decimal imc)
    {
        if (cintura <= 0) throw new ArgumentException("La cintura debe ser mayor a cero.", nameof(cintura));
        if (cadera <= 0) throw new ArgumentException("La cadera debe ser mayor a cero.", nameof(cadera));
        if (imc <= 0) throw new ArgumentException("El IMC debe ser mayor a cero.", nameof(imc));
        return new(cintura, cadera, imc);
    }
}
