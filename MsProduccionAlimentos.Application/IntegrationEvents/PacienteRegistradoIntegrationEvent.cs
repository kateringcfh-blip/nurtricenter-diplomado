namespace MsProduccionAlimentos.Application.IntegrationEvents;

/// <summary>
/// Contrato de integración que este microservicio CONSUME desde ms-pacientes-evaluacion
/// cuando se registra un paciente. Debe coincidir exactamente con el contrato que publica
/// MsPacientesEvaluacion.Application.IntegrationEvents.PacienteRegistradoIntegrationEvent.
/// Routing key: "paciente.registrado"
/// </summary>
public sealed record PacienteRegistradoIntegrationEvent(
    Guid PacienteId,
    string Nombre,
    DateTime FechaRegistro);
