using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Domain;

public class OrdenIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => OrdenId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaOrdenIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var ordenId = OrdenId.De(guid);

        // Assert
        Assert.Equal(guid, ordenId.Valor);
    }

    [Fact]
    public void Equals_DosOrdenIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = OrdenId.De(guid);
        var id2 = OrdenId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosOrdenIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = OrdenId.De(Guid.NewGuid());
        var id2 = OrdenId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void Crear_RetornaOrdenIdConValorNoVacio()
    {
        // Arrange & Act
        var ordenId = OrdenId.Crear();

        // Assert
        Assert.NotEqual(Guid.Empty, ordenId.Valor);
    }
}

public class PaqueteIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PaqueteId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaPaqueteIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var paqueteId = PaqueteId.De(guid);

        // Assert
        Assert.Equal(guid, paqueteId.Valor);
    }

    [Fact]
    public void Equals_DosPaqueteIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = PaqueteId.De(guid);
        var id2 = PaqueteId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosPaqueteIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = PaqueteId.De(Guid.NewGuid());
        var id2 = PaqueteId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}

public class ItemOrdenIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => ItemOrdenId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaItemOrdenIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var itemId = ItemOrdenId.De(guid);

        // Assert
        Assert.Equal(guid, itemId.Valor);
    }

    [Fact]
    public void Equals_DosItemOrdenIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = ItemOrdenId.De(guid);
        var id2 = ItemOrdenId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosItemOrdenIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = ItemOrdenId.De(Guid.NewGuid());
        var id2 = ItemOrdenId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}

public class PorcionIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PorcionId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaPorcionIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var porcionId = PorcionId.De(guid);

        // Assert
        Assert.Equal(guid, porcionId.Valor);
    }

    [Fact]
    public void Equals_DosPorcionIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = PorcionId.De(guid);
        var id2 = PorcionId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosPorcionIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = PorcionId.De(Guid.NewGuid());
        var id2 = PorcionId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}
