using MediatR;

namespace MsProduccionAlimentos.Application.Commands.MarcarItemPreparado;

public record MarcarItemPreparadoCommand(Guid OrdenId, Guid ItemOrdenId, int Cantidad) : IRequest<Unit>;
