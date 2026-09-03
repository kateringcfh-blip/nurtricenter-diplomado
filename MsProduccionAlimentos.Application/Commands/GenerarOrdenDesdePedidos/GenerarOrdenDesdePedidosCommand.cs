using MediatR;

namespace MsProduccionAlimentos.Application.Commands.GenerarOrdenDesdePedidos;

public record PedidoRecetaDto(Guid RecetaId, int Cantidad);

public record PedidoPacienteDto(
    Guid PacienteId,
    string NombrePaciente,
    string DireccionEntrega,
    string NumeroId,
    List<PedidoRecetaDto> Recetas);

public record GenerarOrdenDesdePedidosCommand(
    Guid EncargadoCocinaId,
    DateOnly Fecha,
    List<PedidoPacienteDto> Pedidos) : IRequest<GenerarOrdenDesdePedidosResult>;

public record GenerarOrdenDesdePedidosResult(Guid OrdenId, List<Guid> PaqueteIds);
