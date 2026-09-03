using MsPacientesEvaluacion.Domain.Entities;

namespace MsPacientesEvaluacion.Tests.Domain;

public class ConsultaInicialTests
{
    [Fact]
    public void CalcularImc_Con70kgY175cm_RetornaValorCorrecto()
    {
        // Arrange
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "");
        var consulta = ConsultaInicial.Registrar(DateOnly.FromDateTime(DateTime.Today), 70m, 1.75m, anamnesis);

        // Act
        var imc = consulta.CalcularImc();

        // Assert
        // 70 / (1.75 * 1.75) = 22.857142...
        Assert.True(Math.Abs(imc - 22.8571m) < 0.001m);
    }
}
