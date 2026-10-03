using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Tests.Domain;

public class PacienteIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PacienteId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaPacienteIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var pacienteId = PacienteId.De(guid);

        // Assert
        Assert.Equal(guid, pacienteId.Valor);
    }

    [Fact]
    public void Equals_DosPacienteIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = PacienteId.De(guid);
        var id2 = PacienteId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosPacienteIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = PacienteId.De(Guid.NewGuid());
        var id2 = PacienteId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void Crear_RetornaPacienteIdConValorNoVacio()
    {
        // Arrange & Act
        var pacienteId = PacienteId.Crear();

        // Assert
        Assert.NotEqual(Guid.Empty, pacienteId.Valor);
    }
}

public class AnamnesisIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => AnamnesisId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaAnamnesisIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var anamnesisId = AnamnesisId.De(guid);

        // Assert
        Assert.Equal(guid, anamnesisId.Valor);
    }

    [Fact]
    public void Equals_DosAnamnesisIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = AnamnesisId.De(guid);
        var id2 = AnamnesisId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosAnamnesisIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = AnamnesisId.De(Guid.NewGuid());
        var id2 = AnamnesisId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}

public class ConsultaIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => ConsultaId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaConsultaIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var consultaId = ConsultaId.De(guid);

        // Assert
        Assert.Equal(guid, consultaId.Valor);
    }

    [Fact]
    public void Equals_DosConsultaIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = ConsultaId.De(guid);
        var id2 = ConsultaId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosConsultaIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = ConsultaId.De(Guid.NewGuid());
        var id2 = ConsultaId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}

public class EvaluacionIdTests
{
    [Fact]
    public void De_GuidVacio_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => EvaluacionId.De(Guid.Empty));
    }

    [Fact]
    public void De_GuidValido_RetornaEvaluacionIdConValorCorrecto()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var evaluacionId = EvaluacionId.De(guid);

        // Assert
        Assert.Equal(guid, evaluacionId.Valor);
    }

    [Fact]
    public void Equals_DosEvaluacionIdConMismoGuid_SonIguales()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = EvaluacionId.De(guid);
        var id2 = EvaluacionId.De(guid);

        // Act & Assert
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Equals_DosEvaluacionIdConDistintoGuid_NoSonIguales()
    {
        // Arrange
        var id1 = EvaluacionId.De(Guid.NewGuid());
        var id2 = EvaluacionId.De(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(id1, id2);
    }
}
