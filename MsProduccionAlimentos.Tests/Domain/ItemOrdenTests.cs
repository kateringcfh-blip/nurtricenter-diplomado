using MsProduccionAlimentos.Domain.Entities;

namespace MsProduccionAlimentos.Tests.Domain;

public class ItemOrdenTests
{
    // --- MarcarPreparado ---

    [Fact]
    public void MarcarPreparado_CantidadValida_SumaCantidadPreparada()
    {
        // Arrange
        var item = ItemOrden.Crear(Guid.NewGuid(), 5);

        // Act
        item.MarcarPreparado(3);

        // Assert
        Assert.Equal(3, item.CantidadPreparada);
    }

    [Fact]
    public void MarcarPreparado_ExcedeCantidadRequerida_LanzaInvalidOperationException()
    {
        // Arrange
        var item = ItemOrden.Crear(Guid.NewGuid(), 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => item.MarcarPreparado(3));
    }

    // --- EstaCompleto ---

    [Fact]
    public void EstaCompleto_CuandoCantidadPreparadaIgualARequerida_RetornaTrue()
    {
        // Arrange
        var item = ItemOrden.Crear(Guid.NewGuid(), 4);
        item.MarcarPreparado(4);

        // Act
        var resultado = item.EstaCompleto();

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EstaCompleto_CuandoCantidadPreparadaMenorARequerida_RetornaFalse()
    {
        // Arrange
        var item = ItemOrden.Crear(Guid.NewGuid(), 4);
        item.MarcarPreparado(2);

        // Act
        var resultado = item.EstaCompleto();

        // Assert
        Assert.False(resultado);
    }
}
