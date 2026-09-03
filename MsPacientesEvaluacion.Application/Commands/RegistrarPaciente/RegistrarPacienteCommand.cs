using MediatR;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarPaciente;

public record RegistrarPacienteCommand(
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Email,
    string Direccion,
    string Telefono,
    Guid NutricionistaId) : IRequest<Guid>;
