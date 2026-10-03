using Moq;
using MsProduccionAlimentos.Application.Commands.AgregarPorcion;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class AgregarPorcionCommandHandlerTests
{
    [Fact]
    public async Task Handle_PaqueteExiste_LlamaGuardarCambios()
    {
        // Arrange
        var paqueteId = Guid.NewGuid();
        var ordenId = OrdenId.De(Guid.NewGuid());
        var etiqueta = Etiqueta.Crear("Ana López", "Calle 1", "12345");
        var paquete = Paquete.Armar(Guid.NewGuid(), ordenId, etiqueta, DateOnly.FromDateTime(DateTime.Today));

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AgregarPorcionCommand(paqueteId, Guid.NewGuid(), 2.5m);
        var handler = new AgregarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PaqueteExiste_RetornaGuidNoVacio()
    {
        // Arrange
        var paqueteId = Guid.NewGuid();
        var ordenId = OrdenId.De(Guid.NewGuid());
        var etiqueta = Etiqueta.Crear("Ana López", "Calle 1", "12345");
        var paquete = Paquete.Armar(Guid.NewGuid(), ordenId, etiqueta, DateOnly.FromDateTime(DateTime.Today));

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AgregarPorcionCommand(paqueteId, Guid.NewGuid(), 2.5m);
        var handler = new AgregarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultado);
    }

    [Fact]
    public async Task Handle_PaqueteNoExiste_LanzaInvalidOperationException()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync((Paquete?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new AgregarPorcionCommand(Guid.NewGuid(), Guid.NewGuid(), 1m);
        var handler = new AgregarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
