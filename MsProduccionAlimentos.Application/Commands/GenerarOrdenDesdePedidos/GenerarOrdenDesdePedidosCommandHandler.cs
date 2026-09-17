using MediatR;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Commands.GenerarOrdenDesdePedidos;

public class GenerarOrdenDesdePedidosCommandHandler
    : IRequestHandler<GenerarOrdenDesdePedidosCommand, GenerarOrdenDesdePedidosResult>
{
    private readonly IOrdenRepository _ordenRepository;
    private readonly IPaqueteRepository _paqueteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerarOrdenDesdePedidosCommandHandler(
        IOrdenRepository ordenRepository,
        IPaqueteRepository paqueteRepository,
        IUnitOfWork unitOfWork)
    {
        _ordenRepository = ordenRepository;
        _paqueteRepository = paqueteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenerarOrdenDesdePedidosResult> Handle(
        GenerarOrdenDesdePedidosCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Pedidos is null || request.Pedidos.Count == 0)
            throw new ArgumentException("Debe incluir al menos un pedido para generar la orden.");

        // Paso 1: agrupar recetas de todos los pedidos y sumar cantidades
        var totalesPorReceta = request.Pedidos
            .SelectMany(p => p.Recetas)
            .GroupBy(r => r.RecetaId)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Cantidad));

        // Paso 2: generar una sola OrdenProduccion con los totales
        var lote = $"LOTE-{request.Fecha:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var orden = OrdenProduccion.Generar(request.EncargadoCocinaId, lote, request.Fecha);

        foreach (var (recetaId, cantidadTotal) in totalesPorReceta)
            orden.AgregarItem(recetaId, cantidadTotal);

        // Paso 3 y 4: crear un Paquete por paciente con sus porciones específicas
        var paquetes = new List<Paquete>();

        foreach (var pedido in request.Pedidos)
        {
            var etiqueta = Etiqueta.Crear(pedido.NombrePaciente, pedido.DireccionEntrega, pedido.NumeroId);
            var paquete = Paquete.Armar(pedido.PacienteId, orden.Id, etiqueta, request.Fecha);

            foreach (var receta in pedido.Recetas)
                paquete.AgregarPorcion(receta.RecetaId, receta.Cantidad);

            paquetes.Add(paquete);
        }

        // Pasos 5 y 6: persistir todo — el DbContext rastrea todos los cambios
        await _ordenRepository.Agregar(orden);
        foreach (var paquete in paquetes)
            await _paqueteRepository.Agregar(paquete);

        // Paso 7: una sola llamada confirma todo en una transacción
        await _unitOfWork.GuardarCambios(cancellationToken);

        // Paso 8: devolver resultado
        return new GenerarOrdenDesdePedidosResult(
            orden.Id.Valor,
            paquetes.Select(p => p.Id.Valor).ToList());
    }
}
