namespace MsProduccionAlimentos.Application.Commands.GenerarOrden;

public record GenerarOrdenItemDto(Guid RecetaId, int CantidadRequerida);
