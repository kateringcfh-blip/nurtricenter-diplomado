namespace MsProduccionAlimentos.Domain.ValueObjects;

public sealed record Etiqueta
{
    public string NombrePaciente { get; }
    public string DireccionEntrega { get; }
    public string NumeroId { get; }

    private Etiqueta(string nombrePaciente, string direccionEntrega, string numeroId)
    {
        NombrePaciente = nombrePaciente;
        DireccionEntrega = direccionEntrega;
        NumeroId = numeroId;
    }

    public static Etiqueta Crear(string nombrePaciente, string direccionEntrega, string numeroId)
    {
        if (string.IsNullOrWhiteSpace(nombrePaciente))
            throw new ArgumentException("El nombre del paciente no puede estar vacío.", nameof(nombrePaciente));
        if (string.IsNullOrWhiteSpace(direccionEntrega))
            throw new ArgumentException("La dirección de entrega no puede estar vacía.", nameof(direccionEntrega));
        if (string.IsNullOrWhiteSpace(numeroId))
            throw new ArgumentException("El número de identificación no puede estar vacío.", nameof(numeroId));

        return new(nombrePaciente, direccionEntrega, numeroId);
    }
}
