using MediatR;
using Microsoft.EntityFrameworkCore;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Queries.ObtenerPaquetePorId;

public class ObtenerPaquetePorIdQueryHandler : IRequestHandler<ObtenerPaquetePorIdQuery, PaqueteDto?>
{
    private readonly INurTricenterDbContext _context;

    public ObtenerPaquetePorIdQueryHandler(INurTricenterDbContext context)
    {
        _context = context;
    }

    public async Task<PaqueteDto?> Handle(ObtenerPaquetePorIdQuery request, CancellationToken cancellationToken)
    {
        var paqueteId = PaqueteId.De(request.PaqueteId);

        return await _context.Paquetes
            .Where(p => p.Id == paqueteId)
            .Select(p => new PaqueteDto(
                p.Id.Valor,
                p.PacienteId,
                p.OrdenId.Valor,
                p.Fecha,
                p.Estado.ToString(),
                p.Etiqueta.NombrePaciente,
                p.Porciones.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
