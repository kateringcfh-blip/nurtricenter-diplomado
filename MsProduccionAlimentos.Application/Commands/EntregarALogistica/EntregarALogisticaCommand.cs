using MediatR;

namespace MsProduccionAlimentos.Application.Commands.EntregarALogistica;

public record EntregarALogisticaCommand(Guid PaqueteId) : IRequest<Unit>;
