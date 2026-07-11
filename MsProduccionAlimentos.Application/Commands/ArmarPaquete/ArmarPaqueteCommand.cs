using MediatR;

namespace MsProduccionAlimentos.Application.Commands.ArmarPaquete;

public record ArmarPaqueteCommand(
    Guid PacienteId,
    Guid OrdenId,
    DateOnly Fecha,
    string NombrePaciente,
    string DireccionEntrega,
    string NumeroId) : IRequest<Guid>;
