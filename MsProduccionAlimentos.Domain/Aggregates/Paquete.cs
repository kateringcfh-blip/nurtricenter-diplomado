using MsProduccionAlimentos.Domain.Entities;
using MsProduccionAlimentos.Domain.Enums;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Domain.Aggregates;

public class Paquete
{
    private readonly List<Porcion> _porciones = new();

    public PaqueteId Id { get; }
    public Guid PacienteId { get; }
    public OrdenId OrdenId { get; }
    public DateOnly Fecha { get; }
    public Etiqueta Etiqueta { get; }
    public EstadoPaquete Estado { get; private set; }
    public IReadOnlyList<Porcion> Porciones => _porciones.AsReadOnly();

    private Paquete(PaqueteId id, Guid pacienteId, OrdenId ordenId, Etiqueta etiqueta, DateOnly fecha)
    {
        Id = id;
        PacienteId = pacienteId;
        OrdenId = ordenId;
        Fecha = fecha;
        Etiqueta = etiqueta;
        Estado = EstadoPaquete.EnPreparacion;
    }

    public static Paquete Armar(Guid pacienteId, OrdenId ordenId, Etiqueta etiqueta, DateOnly fecha)
    {
        if (pacienteId == Guid.Empty)
            throw new ArgumentException("PacienteId no puede ser un Guid vacío.", nameof(pacienteId));

        return new(PaqueteId.Crear(), pacienteId, ordenId, etiqueta, fecha);
    }

    public void AgregarPorcion(Guid recetaId, decimal cantidad)
    {
        if (Estado != EstadoPaquete.EnPreparacion)
            throw new InvalidOperationException(
                $"No se pueden agregar porciones a un paquete en estado '{Estado}'.");

        _porciones.Add(Porcion.Crear(recetaId, cantidad));
    }

    public void MarcarListo()
    {
        var sinEnvasar = _porciones.Where(p => !p.EstaEnvasada).ToList();
        if (sinEnvasar.Count != 0)
        {
            var ids = string.Join(", ", sinEnvasar.Select(p => p.Id.Valor));
            throw new InvalidOperationException(
                $"No se puede marcar el paquete como listo: las siguientes porciones no están envasadas: {ids}.");
        }

        Estado = EstadoPaquete.Listo;
    }

    public void EntregarALogistica()
    {
        if (Estado != EstadoPaquete.Listo)
            throw new InvalidOperationException(
                $"No se puede entregar a logística un paquete en estado '{Estado}'. Debe estar en estado 'Listo'.");

        Estado = EstadoPaquete.EntregadoLogistica;
    }
}
