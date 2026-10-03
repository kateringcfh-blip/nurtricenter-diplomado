using Moq;
using MsProduccionAlimentos.Application.Commands.EnvasarPorcion;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class EnvasarPorcionCommandHandlerTests
{
    private static Paquete CrearPaqueteConPorcion(out Guid porcionId)
    {
        var ordenId = OrdenId.De(Guid.NewGuid());
        var etiqueta = Etiqueta.Crear("Ana López", "Calle 1", "12345");
        var paquete = Paquete.Armar(Guid.NewGuid(), ordenId, etiqueta, DateOnly.FromDateTime(DateTime.Today));
        var porcion = paquete.AgregarPorcion(Guid.NewGuid(), 1.5m);
        porcionId = porcion.Id.Valor;
        return paquete;
    }

    [Fact]
    public async Task Handle_PaqueteYPorcionExisten_LlamaGuardarCambios()
    {
        // Arrange
        var paquete = CrearPaqueteConPorcion(out var porcionId);

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new EnvasarPorcionCommand(Guid.NewGuid(), porcionId);
        var handler = new EnvasarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PaqueteNoExiste_LanzaInvalidOperationException()
    {
        // Arrange
        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync((Paquete?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new EnvasarPorcionCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new EnvasarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PorcionNoExisteEnPaquete_LanzaInvalidOperationException()
    {
        // Arrange
        var paquete = CrearPaqueteConPorcion(out _);

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new EnvasarPorcionCommand(Guid.NewGuid(), Guid.NewGuid()); // porcionId inexistente
        var handler = new EnvasarPorcionCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
