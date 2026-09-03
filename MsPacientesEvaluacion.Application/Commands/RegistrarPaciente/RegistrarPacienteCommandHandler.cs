using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarPaciente;

public class RegistrarPacienteCommandHandler : IRequestHandler<RegistrarPacienteCommand, Guid>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPacienteCommandHandler(IPacienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarPacienteCommand request, CancellationToken cancellationToken)
    {
        var contacto = DatosContacto.Crear(request.Email, request.Direccion, request.Telefono);
        var paciente = Paciente.Registrar(
            request.Nombre,
            request.Apellido,
            request.FechaNacimiento,
            contacto,
            request.NutricionistaId);

        await _repository.Agregar(paciente);
        await _unitOfWork.GuardarCambios(cancellationToken);

        return paciente.Id.Valor;
    }
}
