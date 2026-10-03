using Moq;
using MsPacientesEvaluacion.Application.Commands.AsignarNutricionista;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class AsignarNutricionistaCommandHandlerTests
{
    private static Paciente CrearPaciente()
    {
        var contacto = DatosContacto.Crear("ana@test.com", "Calle 1", "555-1234");
        return Paciente.Registrar("Ana", "López", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), contacto, Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_PacienteExiste_LlamaGuardarCambios()
    {
        // Arrange
        var paciente = CrearPaciente();
        var repoMock = new Mock<IPacienteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PacienteId>())).ReturnsAsync(paciente);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AsignarNutricionistaCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new AsignarNutricionistaCommandHandler(repoMock.Object, uowMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        uowMock.Verify(u => u.GuardarCambios(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PacienteNoExiste_LanzaKeyNotFoundException()
    {
        // Arrange
        var repoMock = new Mock<IPacienteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PacienteId>())).ReturnsAsync((Paciente?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var command = new AsignarNutricionistaCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new AsignarNutricionistaCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
