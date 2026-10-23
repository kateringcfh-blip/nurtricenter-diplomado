using MediatR;
using MsPacientesEvaluacion.Application.Interfaces;
using MsPacientesEvaluacion.Application.IntegrationEvents;
using MsPacientesEvaluacion.Domain.Aggregates;
using MsPacientesEvaluacion.Domain.Events;
using MsPacientesEvaluacion.Domain.ValueObjects;

namespace MsPacientesEvaluacion.Application.Commands.RegistrarPaciente;

public class RegistrarPacienteCommandHandler : IRequestHandler<RegistrarPacienteCommand, Guid>
{
    private readonly IPacienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;

    public RegistrarPacienteCommandHandler(
        IPacienteRepository repository,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
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

        // Publicar eventos de integración después de confirmar la persistencia,
        // para no notificar a otros microservicios si el guardado falla.
        foreach (var evento in paciente.EventosOcurridos)
        {
            if (evento is PacienteRegistrado pacienteRegistrado)
            {
                var integrationEvent = new PacienteRegistradoIntegrationEvent(
                    pacienteRegistrado.PacienteId.Valor,
                    pacienteRegistrado.Nombre,
                    pacienteRegistrado.FechaRegistro);

                await _eventPublisher.PublishAsync("paciente.registrado", integrationEvent, cancellationToken);
            }
        }
        paciente.LimpiarEventos();

        return paciente.Id.Valor;
    }
}
