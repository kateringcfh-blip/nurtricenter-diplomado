using Moq;
using MsProduccionAlimentos.Application.Commands.ArmarPaquete;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;

namespace MsProduccionAlimentos.Tests.Application;

public class ArmarPaqueteCommandHandlerTests
{
    [Fact]
    public async Task Handle_CommandValido_LlamaAgregarEnRepositorio()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new ArmarPaqueteCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            "Juan García",
            "Av. Siempreviva 742",
            "DNI-12345678");

        var handler = new ArmarPaqueteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.Agregar(It.IsAny<Paquete>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandValido_LlamaGuardarCambios()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new ArmarPaqueteCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            "María Pérez",
            "Calle Falsa 123",
            "DNI-99999999");

        var handler = new ArmarPaqueteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandValido_RetornaGuidNoVacio()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new ArmarPaqueteCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            "Luis Torres",
            "Ruta 8 km 5",
            "DNI-11111111");

        var handler = new ArmarPaqueteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultado);
    }

    [Fact]
    public async Task Handle_PacienteIdVacio_LanzaArgumentException()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var command = new ArmarPaqueteCommand(
            Guid.Empty,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            "Luis Torres",
            "Ruta 8 km 5",
            "DNI-11111111");

        var handler = new ArmarPaqueteCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }
}
