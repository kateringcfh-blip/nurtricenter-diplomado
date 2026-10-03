using Moq;
using MsProduccionAlimentos.Application.Commands.MarcarItemPreparado;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class MarcarItemPreparadoCommandHandlerTests
{
    private static OrdenProduccion CrearOrdenConItem(out Guid itemOrdenId)
    {
        var recetaId = Guid.NewGuid();
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-MARCAR-001", DateOnly.FromDateTime(DateTime.Today));
        orden.AgregarItem(recetaId, 5);
        itemOrdenId = orden.Items[0].Id.Valor;
        return orden;
    }

    [Fact]
    public async Task Handle_OrdenYItemExisten_LlamaGuardarCambios()
    {
        // Arrange
        var orden = CrearOrdenConItem(out var itemOrdenId);

        var repoMock = new Mock<IOrdenRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<OrdenId>())).ReturnsAsync(orden);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new MarcarItemPreparadoCommand(Guid.NewGuid(), itemOrdenId, 3);
        var handler = new MarcarItemPreparadoCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrdenNoExiste_LanzaInvalidOperationException()
    {
        // Arrange
        var repoMock = new Mock<IOrdenRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<OrdenId>())).ReturnsAsync((OrdenProduccion?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new MarcarItemPreparadoCommand(Guid.NewGuid(), Guid.NewGuid(), 1);
        var handler = new MarcarItemPreparadoCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ItemNoExisteEnOrden_LanzaInvalidOperationException()
    {
        // Arrange
        var orden = CrearOrdenConItem(out _);

        var repoMock = new Mock<IOrdenRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<OrdenId>())).ReturnsAsync(orden);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new MarcarItemPreparadoCommand(Guid.NewGuid(), Guid.NewGuid(), 1); // itemOrdenId inexistente
        var handler = new MarcarItemPreparadoCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
