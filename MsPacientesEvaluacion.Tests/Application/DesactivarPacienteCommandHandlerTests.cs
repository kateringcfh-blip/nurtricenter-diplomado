using Moq;
using MsPacientesEvaluacion.Application.Commands.DesactivarPaciente;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class DesactivarPacienteCommandHandlerTests
{
    private static Paciente CrearPaciente()
    {
        var contacto = DatosContacto.Crear("luis@test.com", "Av. 2", "555-9999");
        return Paciente.Registrar("Luis", "Torres", DateOnly.FromDateTime(DateTime.Today.AddYears(-25)), contacto, Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_PacienteActivo_LlamaGuardarCambios()
    {
        // Arrange
        var paciente = CrearPaciente();
        var repoMock = new Mock<IPacienteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PacienteId>())).ReturnsAsync(paciente);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.GuardarCambios(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new DesactivarPacienteCommand(Guid.NewGuid());
        var handler = new DesactivarPacienteCommandHandler(repoMock.Object, uowMock.Object);

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

        var command = new DesactivarPacienteCommand(Guid.NewGuid());
        var handler = new DesactivarPacienteCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
