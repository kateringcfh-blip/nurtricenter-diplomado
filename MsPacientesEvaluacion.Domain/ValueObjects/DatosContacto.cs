namespace MsPacientesEvaluacion.Domain.ValueObjects;

public sealed record DatosContacto
{
    public string Email { get; }
    public string Direccion { get; }
    public string Telefono { get; }

    private DatosContacto() { Email = null!; Direccion = null!; Telefono = null!; } // EF Core

    private DatosContacto(string email, string direccion, string telefono)
    {
        Email = email;
        Direccion = direccion;
        Telefono = telefono;
    }

    public static DatosContacto Crear(string email, string direccion, string telefono)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("El email no puede estar vacío.", nameof(email));
        if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("La dirección no puede estar vacía.", nameof(direccion));
        if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("El teléfono no puede estar vacío.", nameof(telefono));
        return new(email, direccion, telefono);
    }
}
