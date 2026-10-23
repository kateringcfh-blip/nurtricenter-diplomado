namespace MsPacientesEvaluacion.Application.IntegrationEvents;

/// <summary>
/// Contrato de integración publicado en RabbitMQ cuando se registra un paciente.
/// Es independiente del evento de dominio PacienteRegistrado: usa solo tipos primitivos
/// para no acoplar a otros microservicios con los Value Objects internos de este dominio.
/// Routing key: "paciente.registrado"
/// </summary>
public sealed record PacienteRegistradoIntegrationEvent(
    Guid PacienteId,
    string Nombre,
    DateTime FechaRegistro);
