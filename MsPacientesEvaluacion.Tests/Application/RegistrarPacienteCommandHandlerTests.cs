using Moq;
using MsPacientesEvaluacion.Application.Commands.RegistrarPaciente;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;

namespace MsPacientesEvaluacion.Tests.Application;

public class RegistrarPacienteCommandHandlerTests
{
    private static RegistrarPacienteCommand CrearCommand() =>
        new("Sofía", "Ramírez",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-28)),
            "sofia@test.com", "Calle 10", "555-2222",
            Guid.NewGuid());

    [Fact]
    public async Task Handle_CommandValido_LlamaAgregarEnRepositorio()
    {
        // Arrange
        var repoMock = new Mock<IPacienteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegistrarPacienteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(CrearCommand(), CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.Agregar(It.IsAny<Paciente>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandValido_LlamaGuardarCambios()
    {
        // Arrange
        var repoMock = new Mock<IPacienteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegistrarPacienteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(CrearCommand(), CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandValido_RetornaGuidNoVacio()
    {
        // Arrange
        var repoMock = new Mock<IPacienteRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegistrarPacienteCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        var resultado = await handler.Handle(CrearCommand(), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultado);
    }
}
