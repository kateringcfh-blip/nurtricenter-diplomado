using MediatR;
using MsProduccionAlimentos.Application.DTOs;

namespace MsProduccionAlimentos.Application.Queries.ObtenerPaquetePorId;

public record ObtenerPaquetePorIdQuery(Guid PaqueteId) : IRequest<PaqueteDto?>;
