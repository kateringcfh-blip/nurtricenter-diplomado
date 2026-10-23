namespace MsPacientesEvaluacion.Application.IntegrationEvents;

/// <summary>
/// Contrato de integración que este microservicio CONSUME desde ms-produccion-alimentos
/// cuando se cancela una orden de producción. Debe coincidir exactamente con el contrato
/// que publica MsProduccionAlimentos.Application.IntegrationEvents.OrdenCanceladaIntegrationEvent.
/// Routing key: "orden.cancelada"
/// </summary>
public sealed record OrdenCanceladaIntegrationEvent(
    Guid OrdenId,
    string Motivo,
    DateTime FechaCancelacion);
