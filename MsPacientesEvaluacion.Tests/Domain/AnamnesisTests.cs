using MsPacientesEvaluacion.Domain.Entities;

namespace MsPacientesEvaluacion.Tests.Domain;

public class AnamnesisTests
{
    [Fact]
    public void TieneRestricciones_ConNecesidadesEspecificas_RetornaTrue()
    {
        // Arrange
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "Sin gluten");

        // Act
        var resultado = anamnesis.TieneRestricciones();

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void TieneRestricciones_SinNecesidadesEspecificas_RetornaFalse()
    {
        // Arrange
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "");

        // Act
        var resultado = anamnesis.TieneRestricciones();

        // Assert
        Assert.False(resultado);
    }
}
