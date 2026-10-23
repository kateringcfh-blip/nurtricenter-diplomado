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
    private readonly IIntegrationEventOutbox _outbox;

    public RegistrarPacienteCommandHandler(
        IPacienteRepository repository,
        IUnitOfWork unitOfWork,
        IIntegrationEventOutbox outbox)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
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

        // Patrón Transactional Outbox: el evento de integración se encola en el mismo
        // DbContext que el aggregate, así que se persiste atómicamente junto con el
        // paciente en el GuardarCambios() de abajo. Si el guardado falla, el evento
        // tampoco se encola (no hay publicación "fantasma"); si el guardado tiene éxito,
        // el evento queda garantizado en la tabla OutboxMessages aunque el proceso se
        // caiga antes de llegar a publicarlo a RabbitMQ (lo hace, aparte, el
        // OutboxPublisherBackgroundService).
        foreach (var evento in paciente.EventosOcurridos)
        {
            if (evento is PacienteRegistrado pacienteRegistrado)
            {
                var integrationEvent = new PacienteRegistradoIntegrationEvent(
                    pacienteRegistrado.PacienteId.Valor,
                    pacienteRegistrado.Nombre,
                    pacienteRegistrado.FechaRegistro);

                _outbox.Enqueuear("paciente.registrado", integrationEvent);
            }
        }
        paciente.LimpiarEventos();

        await _unitOfWork.GuardarCambios(cancellationToken);

        return paciente.Id.Valor;
    }
}
