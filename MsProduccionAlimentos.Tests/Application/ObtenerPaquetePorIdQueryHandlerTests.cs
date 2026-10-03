using MockQueryable.Moq;
using Moq;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Application.Queries.ObtenerPaquetePorId;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class ObtenerPaquetePorIdQueryHandlerTests
{
    private static Paquete CrearPaquete()
    {
        var ordenId = OrdenId.De(Guid.NewGuid());
        var etiqueta = Etiqueta.Crear("Carlos Ruiz", "Calle 5", "DNI-55555");
        return Paquete.Armar(Guid.NewGuid(), ordenId, etiqueta, DateOnly.FromDateTime(DateTime.Today));
    }

    [Fact]
    public async Task Handle_PaqueteExiste_RetornaPaqueteDtoConIdCorrecto()
    {
        // Arrange
        var paquete = CrearPaquete();
        var paquetes = new List<Paquete> { paquete }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.Paquetes).Returns(paquetes.Object);

        var handler = new ObtenerPaquetePorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerPaquetePorIdQuery(paquete.Id.Valor), CancellationToken.None);

        // Assert
        Assert.Equal(paquete.Id.Valor, resultado!.Id);
    }

    [Fact]
    public async Task Handle_PaqueteNoExiste_RetornaNull()
    {
        // Arrange
        var paquetes = new List<Paquete>().AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.Paquetes).Returns(paquetes.Object);

        var handler = new ObtenerPaquetePorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerPaquetePorIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(resultado);
    }
}
