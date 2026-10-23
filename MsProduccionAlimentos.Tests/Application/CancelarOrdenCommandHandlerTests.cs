using Moq;
using MsProduccionAlimentos.Application.Commands.CancelarOrden;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class CancelarOrdenCommandHandlerTests
{
    private static OrdenProduccion CrearOrden() =>
        OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-TEST-001", DateOnly.FromDateTime(DateTime.Today));

    [Fact]
    public async Task Handle_OrdenExiste_LlamaGuardarCambios()
    {
        // Arrange
        var orden = CrearOrden();
        var repoMock = new Mock<IOrdenRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<OrdenId>())).ReturnsAsync(orden);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var eventPublisherMock = new Mock<IEventPublisher>();

        var command = new CancelarOrdenCommand(Guid.NewGuid(), "Motivo de cancelación");
        var handler = new CancelarOrdenCommandHandler(repoMock.Object, uowMock.Object, eventPublisherMock.Object);

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
        var eventPublisherMock = new Mock<IEventPublisher>();

        var command = new CancelarOrdenCommand(Guid.NewGuid(), "Motivo");
        var handler = new CancelarOrdenCommandHandler(repoMock.Object, uowMock.Object, eventPublisherMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OrdenExiste_PublicaEventoOrdenCancelada()
    {
        // Arrange
        var orden = CrearOrden();
        var repoMock = new Mock<IOrdenRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<OrdenId>())).ReturnsAsync(orden);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var eventPublisherMock = new Mock<IEventPublisher>();

        var command = new CancelarOrdenCommand(Guid.NewGuid(), "Motivo de cancelación");
        var handler = new CancelarOrdenCommandHandler(repoMock.Object, uowMock.Object, eventPublisherMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: se publicó exactamente un evento de integración con la routing key esperada
        eventPublisherMock.Verify(
            p => p.PublishAsync("orden.cancelada", It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
