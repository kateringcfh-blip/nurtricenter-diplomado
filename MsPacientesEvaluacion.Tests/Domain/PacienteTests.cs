using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.Entities;
using MsPacientesEvaluacion.Domain.Enums;
using MsPacientesEvaluacion.Domain.Events;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Domain;

public class PacienteTests
{
    private static DatosContacto ContactoValido() =>
        DatosContacto.Crear("ana@ejemplo.com", "Calle 10 #5-20", "3001234567");

    // --- Registrar ---

    [Fact]
    public void Registrar_ConDatosValidos_CreaEnEstadoActivo()
    {
        // Arrange
        var contacto = ContactoValido();

        // Act
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), contacto, Guid.NewGuid());

        // Assert
        Assert.Equal(EstadoPaciente.Activo, paciente.Estado);
    }

    [Fact]
    public void Registrar_ConDatosValidos_PublicaEventoPacienteRegistrado()
    {
        // Arrange
        var contacto = ContactoValido();

        // Act
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), contacto, Guid.NewGuid());

        // Assert
        Assert.Contains(paciente.EventosOcurridos, e => e is PacienteRegistrado);
    }

    // --- RegistrarConsultaInicial ---

    [Fact]
    public void RegistrarConsultaInicial_SinConsultaPrevia_AsignaConsulta()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "");

        // Act
        paciente.RegistrarConsultaInicial(DateOnly.FromDateTime(DateTime.Today), 65m, 1.65m, anamnesis);

        // Assert
        Assert.NotNull(paciente.ConsultaInicial);
    }

    [Fact]
    public void RegistrarConsultaInicial_SinConsultaPrevia_CambiaEstadoAEnEvaluacion()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "");

        // Act
        paciente.RegistrarConsultaInicial(DateOnly.FromDateTime(DateTime.Today), 65m, 1.65m, anamnesis);

        // Assert
        Assert.Equal(EstadoPaciente.EnEvaluacion, paciente.Estado);
    }

    [Fact]
    public void RegistrarConsultaInicial_ConConsultaExistente_LanzaInvalidOperationException()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());
        var anamnesis = Anamnesis.Registrar("Dieta variada", "Sin antecedentes", "");
        paciente.RegistrarConsultaInicial(DateOnly.FromDateTime(DateTime.Today), 65m, 1.65m, anamnesis);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            paciente.RegistrarConsultaInicial(DateOnly.FromDateTime(DateTime.Today), 70m, 1.65m, anamnesis));
    }

    // --- Desactivar ---

    [Fact]
    public void Desactivar_CuandoEstaActivo_CambiaEstadoAInactivo()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());

        // Act
        paciente.Desactivar();

        // Assert
        Assert.Equal(EstadoPaciente.Inactivo, paciente.Estado);
    }

    [Fact]
    public void Desactivar_CuandoYaEstaInactivo_LanzaInvalidOperationException()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());
        paciente.Desactivar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => paciente.Desactivar());
    }

    // --- RegistrarEvaluacion ---

    [Fact]
    public void RegistrarEvaluacion_CuandoActivo_AgregaEvaluacion()
    {
        // Arrange
        var paciente = Paciente.Registrar("Ana", "López", new DateOnly(1990, 5, 15), ContactoValido(), Guid.NewGuid());
        var medidas = Medidas.Crear(80m, 95m, 23.9m);

        // Act
        paciente.RegistrarEvaluacion(DateOnly.FromDateTime(DateTime.Today), 65m, medidas, "Progreso normal", NivelAdherencia.Alta, Guid.NewGuid());

        // Assert
        Assert.Single(paciente.Evaluaciones);
    }
}
