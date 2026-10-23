namespace MsProduccionAlimentos.Application.Interfaces;

/// <summary>
/// Puerto de salida para publicar eventos de integración hacia otros microservicios.
/// La implementación concreta (RabbitMQ) vive en la capa de Infrastructure.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object integrationEvent, CancellationToken cancellationToken = default);
}
