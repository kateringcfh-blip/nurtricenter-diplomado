using MediatR;

namespace MsProduccionAlimentos.Application.Commands.GenerarOrden;

public record GenerarOrdenCommand(
    Guid EncargadoCocinaId,
    string LoteProduccion,
    DateOnly Fecha,
    List<GenerarOrdenItemDto> Items) : IRequest<Guid>;
