using MockQueryable.Moq;
using Moq;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Application.Queries.ListarPacientes;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Application;

public class ListarPacientesQueryHandlerTests
{
    private static Paciente CrearPaciente(string nombre)
    {
        var contacto = DatosContacto.Crear($"{nombre.ToLower()}@test.com", "Calle 1", "555-0000");
        return Paciente.Registrar(nombre, "Apellido", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), contacto, Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_SinPacientes_RetornaListaVacia()
    {
        // Arrange
        var pacientes = new List<Paciente>().AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IPacientesDbContext>();
        contextMock.Setup(c => c.Pacientes).Returns(pacientes.Object);

        var handler = new ListarPacientesQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ListarPacientesQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Handle_ConDoPacientes_RetornaDosDto()
    {
        // Arrange
        var lista = new List<Paciente> { CrearPaciente("Ana"), CrearPaciente("Luis") };
        var pacientes = lista.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IPacientesDbContext>();
        contextMock.Setup(c => c.Pacientes).Returns(pacientes.Object);

        var handler = new ListarPacientesQueryHandler(contextMock.Object);

        // Act
        var resultado = await handler.Handle(new ListarPacientesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, resultado.Count);
    }
}
