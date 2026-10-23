namespace MsProduccionAlimentos.Application.IntegrationEvents;

/// <summary>
/// Contrato de integración publicado en RabbitMQ cuando se cancela una orden de producción.
/// Es independiente del evento de dominio OrdenCancelada: usa solo tipos primitivos
/// para no acoplar a otros microservicios con los Value Objects internos de este dominio.
/// Routing key: "orden.cancelada"
/// </summary>
public sealed record OrdenCanceladaIntegrationEvent(
    Guid OrdenId,
    string Motivo,
    DateTime FechaCancelacion);
