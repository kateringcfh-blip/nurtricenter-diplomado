using MediatR;
using MsProduccionAlimentos.Application.DTOs;

namespace MsProduccionAlimentos.Application.Queries.ObtenerOrdenPorId;

public record ObtenerOrdenPorIdQuery(Guid OrdenId) : IRequest<OrdenDto?>;
