using MockQueryable.Moq;
using Moq;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Application.Queries.ListarOrdenes;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Tests.Application;

public class ListarOrdenesQueryHandlerTests
{
    [Fact]
    public async Task Handle_SinOrdenes_RetornaListaVacia()
    {
        // Arrange
        var ordenes = new List<OrdenProduccion>().AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.OrdenesProd).Returns(ordenes.Object);

        var handler = new ListarOrdenesQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ListarOrdenesQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Handle_ConUnaOrden_RetornaUnaOrdenDto()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-LISTAR-001", DateOnly.FromDateTime(DateTime.Today));
        var ordenes = new List<OrdenProduccion> { orden }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<INurTricenterDbContext>();
        contextMock.Setup(c => c.OrdenesProd).Returns(ordenes.Object);

        var handler = new ListarOrdenesQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ListarOrdenesQuery(), CancellationToken.None);

        // Assert
        Assert.Single(resultado);
    }
}
