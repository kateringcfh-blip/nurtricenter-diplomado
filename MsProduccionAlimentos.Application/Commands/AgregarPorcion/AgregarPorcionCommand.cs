using MediatR;

namespace MsProduccionAlimentos.Application.Commands.AgregarPorcion;

public record AgregarPorcionCommand(Guid PaqueteId, Guid RecetaId, decimal Cantidad) : IRequest<Guid>;
