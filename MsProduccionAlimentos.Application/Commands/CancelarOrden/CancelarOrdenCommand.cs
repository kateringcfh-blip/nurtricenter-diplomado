using MediatR;

namespace MsProduccionAlimentos.Application.Commands.CancelarOrden;

public record CancelarOrdenCommand(Guid OrdenId, string Motivo) : IRequest<Unit>;
