using Moq;
using MsPacientesEvaluacion.Application.Commands.RegistrarEvaluacion;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class RegistrarEvaluacionCommandHandlerTests
{
    private static Paciente CrearPaciente()
    {
        var contacto = DatosContacto.Crear("carlos@test.com", "Av. 5", "555-7777");
        return Paciente.Registrar("Carlos", "Mendoza", DateOnly.FromDateTime(DateTime.Today.AddYears(-35)), contacto, Guid.NewGuid());
    }

    private static RegistrarEvaluacionCommand CrearCommand(Guid pacienteId) =>
        new(pacienteId,
            DateOnly.FromDateTime(DateTime.Today),
            75m,
            85m, 95m, 24.5m,
            "Sin observaciones",
            "Alta",
            Guid.NewGuid());

    [Fact]
    public async Task Handle_PacienteExiste_LlamaGuardarCambios()
    {
        // Arrange
        var paciente = CrearPaciente();
        var repoMock = new Mock<IPacienteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PacienteId>())).ReturnsAsync(paciente);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = CrearCommand(Guid.NewGuid());
        var handler = new RegistrarEvaluacionCommandHandler(repoMock.Object, uowMock.Object);

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

        var command = CrearCommand(Guid.NewGuid());
        var handler = new RegistrarEvaluacionCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
