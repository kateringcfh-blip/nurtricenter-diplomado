namespace MsProduccionAlimentos.Application.Interfaces;

/// <summary>
/// Puerto de salida para publicar eventos de integración hacia otros microservicios.
/// La implementación concreta (RabbitMQ) vive en la capa de Infrastructure.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object integrationEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publica un payload JSON ya serializado tal cual, sin volver a serializar un objeto .NET.
    /// La usa el OutboxPublisherBackgroundService, que solo tiene el JSON guardado en la tabla
    /// OutboxMessages (patrón Transactional Outbox), no la instancia original del evento.
    /// </summary>
    Task PublishRawAsync(string routingKey, string jsonPayload, CancellationToken cancellationToken = default);
}
