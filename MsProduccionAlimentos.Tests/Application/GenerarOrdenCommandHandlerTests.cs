using Moq;
using MsProduccionAlimentos.Application.Commands.GenerarOrden;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Tests.Application;

public class GenerarOrdenCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConItemsValidos_AgregaOrdenAlRepositorio()
    {
        // Arrange
        var repoMock = new Mock<IOrdenRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var recetaId = Guid.NewGuid();
        var command = new GenerarOrdenCommand(
            Guid.NewGuid(),
            "LOTE-TEST-001",
            DateOnly.FromDateTime(DateTime.Today),
            new List<GenerarOrdenItemDto>
            {
                new(recetaId, 3)
            });

        var handler = new GenerarOrdenCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.Agregar(It.IsAny<OrdenProduccion>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConItemsValidos_LlamaGuardarCambiosExactamenteUnaVez()
    {
        // Arrange
        var repoMock = new Mock<IOrdenRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new GenerarOrdenCommand(
            Guid.NewGuid(),
            "LOTE-TEST-002",
            DateOnly.FromDateTime(DateTime.Today),
            new List<GenerarOrdenItemDto>
            {
                new(Guid.NewGuid(), 2),
                new(Guid.NewGuid(), 4)
            });

        var handler = new GenerarOrdenCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConItemsValidos_RetornaGuidNoVacio()
    {
        // Arrange
        var repoMock = new Mock<IOrdenRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new GenerarOrdenCommand(
            Guid.NewGuid(),
            "LOTE-TEST-003",
            DateOnly.FromDateTime(DateTime.Today),
            new List<GenerarOrdenItemDto> { new(Guid.NewGuid(), 1) });

        var handler = new GenerarOrdenCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultado);
    }
}
