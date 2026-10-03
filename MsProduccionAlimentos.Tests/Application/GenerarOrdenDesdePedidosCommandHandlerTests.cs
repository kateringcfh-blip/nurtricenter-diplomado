using Moq;
using MsProduccionAlimentos.Application.Commands.GenerarOrdenDesdePedidos;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Tests.Application;

public class GenerarOrdenDesdePedidosCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConDosPedidos_LlamaAgregarEnRepoOrden()
    {
        // Arrange
        var ordenRepoMock = new Mock<IOrdenRepository>();
        var paqueteRepoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new GenerarOrdenDesdePedidosCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new List<PedidoPacienteDto>
            {
                new(Guid.NewGuid(), "Ana García", "Calle 1", "DNI-1", new List<PedidoRecetaDto> { new(Guid.NewGuid(), 2) }),
                new(Guid.NewGuid(), "Luis Torres", "Calle 2", "DNI-2", new List<PedidoRecetaDto> { new(Guid.NewGuid(), 3) })
            });

        var handler = new GenerarOrdenDesdePedidosCommandHandler(ordenRepoMock.Object, paqueteRepoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        ordenRepoMock.Verify(r => r.Agregar(It.IsAny<OrdenProduccion>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConDosPedidos_LlamaAgregarEnRepoPaqueteDosveces()
    {
        // Arrange
        var ordenRepoMock = new Mock<IOrdenRepository>();
        var paqueteRepoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new GenerarOrdenDesdePedidosCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new List<PedidoPacienteDto>
            {
                new(Guid.NewGuid(), "Ana García", "Calle 1", "DNI-1", new List<PedidoRecetaDto> { new(Guid.NewGuid(), 1) }),
                new(Guid.NewGuid(), "Luis Torres", "Calle 2", "DNI-2", new List<PedidoRecetaDto> { new(Guid.NewGuid(), 2) })
            });

        var handler = new GenerarOrdenDesdePedidosCommandHandler(ordenRepoMock.Object, paqueteRepoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        paqueteRepoMock.Verify(r => r.Agregar(It.IsAny<Paquete>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ListaPedidosVacia_LanzaArgumentException()
    {
        // Arrange
        var ordenRepoMock = new Mock<IOrdenRepository>();
        var paqueteRepoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var command = new GenerarOrdenDesdePedidosCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new List<PedidoPacienteDto>());

        var handler = new GenerarOrdenDesdePedidosCommandHandler(ordenRepoMock.Object, paqueteRepoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ConUnPedido_RetornaOrdenIdNoVacio()
    {
        // Arrange
        var ordenRepoMock = new Mock<IOrdenRepository>();
        var paqueteRepoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new GenerarOrdenDesdePedidosCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new List<PedidoPacienteDto>
            {
                new(Guid.NewGuid(), "María Ruiz", "Av. Libertad 10", "DNI-99", new List<PedidoRecetaDto> { new(Guid.NewGuid(), 1) })
            });

        var handler = new GenerarOrdenDesdePedidosCommandHandler(ordenRepoMock.Object, paqueteRepoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultado.OrdenId);
    }
}
