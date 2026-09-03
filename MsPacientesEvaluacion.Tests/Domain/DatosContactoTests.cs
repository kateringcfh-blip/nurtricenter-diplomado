using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Domain;

public class DatosContactoTests
{
    [Fact]
    public void Crear_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange & Act
        var contacto = DatosContacto.Crear("usuario@ejemplo.com", "Calle 10 #5-20", "3001234567");

        // Assert
        Assert.Equal("usuario@ejemplo.com", contacto.Email);
    }

    [Fact]
    public void Crear_EmailVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => DatosContacto.Crear("", "Calle 10 #5-20", "3001234567"));
    }
}
