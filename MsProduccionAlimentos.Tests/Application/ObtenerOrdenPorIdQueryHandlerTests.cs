using MockQueryable.Moq;
using Moq;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Application.Queries.ObtenerOrdenPorId;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Tests.Application;

public class ObtenerOrdenPorIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_OrdenExiste_RetornaOrdenDtoConIdCorrecto()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-OBTENER-001", DateOnly.FromDateTime(DateTime.Today));
        var ordenes = new List<OrdenProduccion> { orden }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.OrdenesProd).Returns(ordenes.Object);

        var handler = new ObtenerOrdenPorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerOrdenPorIdQuery(orden.Id.Valor), CancellationToken.None);

        // Assert
        Assert.Equal(orden.Id.Valor, resultado!.Id);
    }

    [Fact]
    public async Task Handle_OrdenNoExiste_RetornaNull()
    {
        // Arrange
        var ordenes = new List<OrdenProduccion>().AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.OrdenesProd).Returns(ordenes.Object);

        var handler = new ObtenerOrdenPorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerOrdenPorIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(resultado);
    }
}
