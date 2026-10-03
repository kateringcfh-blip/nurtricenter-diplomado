using MockQueryable.Moq;
using Moq;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Application.Queries.ObtenerPacientePorId;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class ObtenerPacientePorIdQueryHandlerTests
{
    private static Paciente CrearPaciente()
    {
        var contacto = DatosContacto.Crear("elena@test.com", "Av. 9", "555-8888");
        return Paciente.Registrar("Elena", "Vásquez", DateOnly.FromDateTime(DateTime.Today.AddYears(-45)), contacto, Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_PacienteExiste_RetornaPacienteDetalleDtoConIdCorrecto()
    {
        // Arrange
        var paciente = CrearPaciente();
        var pacientes = new List<Paciente> { paciente }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IPacientesDbContext>();
        contextMock.Setup(c => c.Pacientes).Returns(pacientes.Object);

        var handler = new ObtenerPacientePorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerPacientePorIdQuery(paciente.Id.Valor), CancellationToken.None);

        // Assert
        Assert.Equal(paciente.Id.Valor, resultado!.Id);
    }

    [Fact]
    public async Task Handle_PacienteNoExiste_RetornaNull()
    {
        // Arrange
        var pacientes = new List<Paciente>().AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IPacientesDbContext>();
        contextMock.Setup(c => c.Pacientes).Returns(pacientes.Object);

        var handler = new ObtenerPacientePorIdQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ObtenerPacientePorIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(resultado);
    }
}
