namespace MsProduccionAlimentos.Application.DTOs;

public record OrdenDto(
    Guid Id,
    DateOnly Fecha,
    string Estado,
    string LoteProduccion,
    int TotalItems,
    int ItemsCompletos);
