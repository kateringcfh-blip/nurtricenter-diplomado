using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Tests.Domain;

public class PaqueteTests
{
    private static Etiqueta EtiquetaValida() =>
        Etiqueta.Crear("Juan Pérez", "Av. Principal 123", "12345678");

    private static OrdenId OrdenIdValido() => OrdenId.Crear();

    // --- Armar ---

    [Fact]
    public void Armar_ConDatosValidos_CreaEnEstadoEnPreparacion()
    {
        // Arrange
        var pacienteId = Guid.NewGuid();
        var ordenId = OrdenIdValido();
        var etiqueta = EtiquetaValida();
        var fecha = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var paquete = Paquete.Armar(pacienteId, ordenId, etiqueta, fecha);

        // Assert
        Assert.Equal(EstadoPaquete.EnPreparacion, paquete.Estado);
    }

    // --- AgregarPorcion ---

    [Fact]
    public void AgregarPorcion_CuandoEstaEnPreparacion_AgregaPorcion()
    {
        // Arrange
        var paquete = Paquete.Armar(Guid.NewGuid(), OrdenIdValido(), EtiquetaValida(), DateOnly.FromDateTime(DateTime.Today));

        // Act
        paquete.AgregarPorcion(Guid.NewGuid(), 100m);

        // Assert
        Assert.Single(paquete.Porciones);
    }

    [Fact]
    public void AgregarPorcion_CuandoNoEstaEnPreparacion_LanzaInvalidOperationException()
    {
        // Arrange
        var paquete = Paquete.Armar(Guid.NewGuid(), OrdenIdValido(), EtiquetaValida(), DateOnly.FromDateTime(DateTime.Today));
        var porcion = paquete.AgregarPorcion(Guid.NewGuid(), 50m);
        porcion.Envasar();
        paquete.MarcarListo();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => paquete.AgregarPorcion(Guid.NewGuid(), 30m));
    }

    // --- MarcarListo ---

    [Fact]
    public void MarcarListo_CuandoTodasLasPorcionesEnvasadas_CambiaEstadoAListo()
    {
        // Arrange
        var paquete = Paquete.Armar(Guid.NewGuid(), OrdenIdValido(), EtiquetaValida(), DateOnly.FromDateTime(DateTime.Today));
        var porcion = paquete.AgregarPorcion(Guid.NewGuid(), 200m);
        porcion.Envasar();

        // Act
        paquete.MarcarListo();

        // Assert
        Assert.Equal(EstadoPaquete.Listo, paquete.Estado);
    }

    [Fact]
    public void MarcarListo_CuandoHayPorcionSinEnvasar_LanzaInvalidOperationException()
    {
        // Arrange
        var paquete = Paquete.Armar(Guid.NewGuid(), OrdenIdValido(), EtiquetaValida(), DateOnly.FromDateTime(DateTime.Today));
        paquete.AgregarPorcion(Guid.NewGuid(), 200m); // sin envasar

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => paquete.MarcarListo());
    }

    // --- EntregarALogistica ---

    [Fact]
    public void EntregarALogistica_CuandoEstaListo_CambiaEstadoAEntregadoLogistica()
    {
        // Arrange
        var paquete = Paquete.Armar(Guid.NewGuid(), OrdenIdValido(), EtiquetaValida(), DateOnly.FromDateTime(DateTime.Today));
        var porcion = paquete.AgregarPorcion(Guid.NewGuid(), 150m);
        porcion.Envasar();
        paquete.MarcarListo();

        // Act
        paquete.EntregarALogistica();

        // Assert
        Assert.Equal(EstadoPaquete.EntregadoLogistica, paquete.Estado);
    }
}
