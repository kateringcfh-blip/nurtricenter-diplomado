using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Domain;

public class EtiquetaTests
{
    [Fact]
    public void Crear_ConDatosValidos_CreaEtiquetaCorrectamente()
    {
        // Arrange & Act
        var etiqueta = Etiqueta.Crear("María García", "Calle 45 #10-20", "98765432");

        // Assert
        Assert.Equal("María García", etiqueta.NombrePaciente);
    }

    [Fact]
    public void Crear_NombrePacienteVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Etiqueta.Crear("", "Calle 45 #10-20", "98765432"));
    }
}
