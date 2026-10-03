using Moq;
using MsProduccionAlimentos.Application.Commands.EntregarALogistica;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Application;

public class EntregarALogisticaCommandHandlerTests
{
    private static Paquete CrearPaqueteSinPorciones()
    {
        var ordenId = OrdenId.De(Guid.NewGuid());
        var etiqueta = Etiqueta.Crear("Ana López", "Calle 1", "12345");
        return Paquete.Armar(Guid.NewGuid(), ordenId, etiqueta, DateOnly.FromDateTime(DateTime.Today));
    }

    [Fact]
    public async Task Handle_PaqueteExisteYListo_LlamaGuardarCambios()
    {
        // Arrange
        var paquete = CrearPaqueteSinPorciones();
        // Paquete sin porciones puede ser marcado listo
        paquete.MarcarListo();

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new EntregarALogisticaCommand(Guid.NewGuid());
        var handler = new EntregarALogisticaCommandHandler(repoMock.Object, uowMock.Object);

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

        var command = new EntregarALogisticaCommand(Guid.NewGuid());
        var handler = new EntregarALogisticaCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PaqueteConPorcionSinEnvasar_LanzaInvalidOperationException()
    {
        // Arrange — una porción sin envasar impide que MarcarListo() complete
        var paquete = CrearPaqueteSinPorciones();
        paquete.AgregarPorcion(Guid.NewGuid(), 1); // porción no envasada

        var repoMock = new Mock<IPaqueteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PaqueteId>())).ReturnsAsync(paquete);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new EntregarALogisticaCommand(Guid.NewGuid());
        var handler = new EntregarALogisticaCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
