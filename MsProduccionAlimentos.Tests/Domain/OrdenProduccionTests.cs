using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Domain.Events;

namespace MsProduccionAlimentos.Tests.Domain;

public class OrdenProduccionTests
{
    // --- Generar ---

    [Fact]
    public void Generar_ConDatosValidos_CreaOrdenEnEstadoPendiente()
    {
        // Arrange
        var encargadoId = Guid.NewGuid();
        var lote = "LOTE-001";
        var fecha = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var orden = OrdenProduccion.Generar(encargadoId, lote, fecha);

        // Assert
        Assert.Equal(EstadoOrden.Pendiente, orden.Estado);
    }

    // --- AgregarItem ---

    [Fact]
    public void AgregarItem_CuandoEstaPendiente_AgregaItemCorrectamente()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));
        var recetaId = Guid.NewGuid();

        // Act
        orden.AgregarItem(recetaId, 5);

        // Assert
        Assert.Single(orden.Items);
    }

    [Fact]
    public void AgregarItem_CuandoNoEstaPendiente_LanzaInvalidOperationException()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));
        orden.Cancelar("motivo de prueba");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => orden.AgregarItem(Guid.NewGuid(), 3));
    }

    // --- Cancelar ---

    [Fact]
    public void Cancelar_CuandoEstaCompletada_LanzaInvalidOperationException()
    {
        // Arrange
        var recetaId = Guid.NewGuid();
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));
        orden.AgregarItem(recetaId, 1);
        orden.MarcarItemPreparado(orden.Items[0].Id, 1);
        orden.Completar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => orden.Cancelar("motivo"));
    }

    [Fact]
    public void Cancelar_CuandoEstaPendiente_CambiaEstadoACancelada()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));

        // Act
        orden.Cancelar("motivo de cancelación");

        // Assert
        Assert.Equal(EstadoOrden.Cancelada, orden.Estado);
    }

    [Fact]
    public void Cancelar_CuandoEstaPendiente_PublicaEventoOrdenCancelada()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));

        // Act
        orden.Cancelar("motivo de cancelación");

        // Assert
        Assert.Contains(orden.EventosOcurridos, e => e is OrdenCancelada);
    }

    // --- Completar ---

    [Fact]
    public void Completar_CuandoTodosLosItemsCompletos_CambiaEstadoACompletada()
    {
        // Arrange
        var recetaId = Guid.NewGuid();
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));
        orden.AgregarItem(recetaId, 2);
        orden.MarcarItemPreparado(orden.Items[0].Id, 2);

        // Act
        orden.Completar();

        // Assert
        Assert.Equal(EstadoOrden.Completada, orden.Estado);
    }

    [Fact]
    public void Completar_CuandoHayItemsIncompletos_LanzaInvalidOperationException()
    {
        // Arrange
        var orden = OrdenProduccion.Generar(Guid.NewGuid(), "LOTE-001", DateOnly.FromDateTime(DateTime.Today));
        orden.AgregarItem(Guid.NewGuid(), 3);
        // Item agregado pero no preparado

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => orden.Completar());
    }
}
