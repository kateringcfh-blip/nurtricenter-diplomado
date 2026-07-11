using MediatR;
using MsProduccionAlimentos.Application.DTOs;

namespace MsProduccionAlimentos.Application.Queries.ListarOrdenes;

public record ListarOrdenesQuery : IRequest<List<OrdenDto>>;
