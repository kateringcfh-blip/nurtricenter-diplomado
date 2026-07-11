using MediatR;
using MsProduccionAlimentos.Application.DTOs;
using MsProduccionAlimentos.Application.Interfaces;
using MsProduccionAlimentos.Domain.Aggregates;
using MsProduccionAlimentos.Domain.ValueObjects;

namespace MsProduccionAlimentos.Application.Queries.ObtenerPaquetePorId;

public class ObtenerPaquetePorIdQueryHandler : IRequestHandler<ObtenerPaquetePorIdQuery, PaqueteDto?>
{
    private readonly IPaqueteRepository _repository;

    public ObtenerPaquetePorIdQueryHandler(IPaqueteRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaqueteDto?> Handle(ObtenerPaquetePorIdQuery request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.ObtenerPorId(PaqueteId.De(request.PaqueteId));
        return paquete is null ? null : MapearADto(paquete);
    }

    private static PaqueteDto MapearADto(Paquete paquete) => new(
        Id: paquete.Id.Valor,
        PacienteId: paquete.PacienteId,
        OrdenId: paquete.OrdenId.Valor,
        Fecha: paquete.Fecha,
        Estado: paquete.Estado.ToString(),
        NombrePaciente: paquete.Etiqueta.NombrePaciente,
        TotalPorciones: paquete.Porciones.Count);
}
