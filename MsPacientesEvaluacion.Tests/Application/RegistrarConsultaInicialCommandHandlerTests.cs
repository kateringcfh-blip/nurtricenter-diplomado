using Moq;
using MsPacientesEvaluacion.Application.Commands.RegistrarConsultaInicial;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class RegistrarConsultaInicialCommandHandlerTests
{
    private static Paciente CrearPaciente()
    {
        var contacto = DatosContacto.Crear("maria@test.com", "Calle 3", "555-0001");
        return Paciente.Registrar("María", "García", DateOnly.FromDateTime(DateTime.Today.AddYears(-40)), contacto, Guid.NewGuid());
    }

    private static RegistrarConsultaInicialCommand CrearCommand(Guid pacienteId) =>
        new(pacienteId,
            DateOnly.FromDateTime(DateTime.Today),
            70m, 1.65m,
            "Dieta balanceada",
            "Sin antecedentes",
            "Sin necesidades especiales");

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
        var handler = new RegistrarConsultaInicialCommandHandler(repoMock.Object, uowMock.Object);

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
        var handler = new RegistrarConsultaInicialCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PacienteConConsultaExistente_LanzaInvalidOperationException()
    {
        // Arrange
        var paciente = CrearPaciente();
        // Registrar primera consulta
        var anamnesis = MsPacientesEvaluacion.Domain.Entities.Anamnesis.Registrar("Dieta", "Sin antecedentes", "Ninguna");
        paciente.RegistrarConsultaInicial(DateOnly.FromDateTime(DateTime.Today), 70m, 1.65m, anamnesis);

        var repoMock = new Mock<IPacienteRepository>();
        repoMock.Setup(r => r.ObtenerPorId(It.IsAny<PacienteId>())).ReturnsAsync(paciente);

        var uowMock = new Mock<IUnitOfWork>();

        var command = CrearCommand(Guid.NewGuid());
        var handler = new RegistrarConsultaInicialCommandHandler(repoMock.Object, uowMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
