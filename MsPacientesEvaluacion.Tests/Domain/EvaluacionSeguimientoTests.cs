using MsPacientesEvaluacion.Domain.Entities;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Domain;

public class EvaluacionSeguimientoTests
{
    private static Medidas MedidasValidas() => Medidas.Crear(80m, 95m, 23m);

    [Fact]
    public void CompararConAnterior_PesoMenor_RetornaMejora()
    {
        // Arrange
        var anterior = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 1, 1), 75m, MedidasValidas(), "Observacion", NivelAdherencia.Alta, Guid.NewGuid());
        var actual = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 2, 1), 70m, MedidasValidas(), "Observacion", NivelAdherencia.Alta, Guid.NewGuid());

        // Act
        var resultado = actual.CompararConAnterior(anterior);

        // Assert
        Assert.Equal("MEJORA", resultado);
    }

    [Fact]
    public void CompararConAnterior_PesoIgual_RetornaEstable()
    {
        // Arrange
        var anterior = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 1, 1), 70m, MedidasValidas(), "Observacion", NivelAdherencia.Media, Guid.NewGuid());
        var actual = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 2, 1), 70m, MedidasValidas(), "Observacion", NivelAdherencia.Media, Guid.NewGuid());

        // Act
        var resultado = actual.CompararConAnterior(anterior);

        // Assert
        Assert.Equal("ESTABLE", resultado);
    }

    [Fact]
    public void CompararConAnterior_PesoMayor_RetornaRetroceso()
    {
        // Arrange
        var anterior = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 1, 1), 68m, MedidasValidas(), "Observacion", NivelAdherencia.Baja, Guid.NewGuid());
        var actual = EvaluacionSeguimiento.Registrar(
            new DateOnly(2026, 2, 1), 72m, MedidasValidas(), "Observacion", NivelAdherencia.Baja, Guid.NewGuid());

        // Act
        var resultado = actual.CompararConAnterior(anterior);

        // Assert
        Assert.Equal("RETROCESO", resultado);
    }
}
