using MediatR;

namespace MsProduccionAlimentos.Application.Commands.EnvasarPorcion;

public record EnvasarPorcionCommand(Guid PaqueteId, Guid PorcionId) : IRequest<Unit>;
