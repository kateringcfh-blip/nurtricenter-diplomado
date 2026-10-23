namespace MsProduccionAlimentos.Application.Interfaces;

/// <summary>
/// Puerto de salida del patrón Transactional Outbox. A diferencia de IEventPublisher
/// (que publica directo a RabbitMQ), esto solo encola el evento junto con el resto de
/// los cambios del DbContext actual: se persiste atómicamente con el aggregate en el
/// mismo _unitOfWork.GuardarCambios(). La publicación real al broker la hace, por
/// separado, OutboxPublisherBackgroundService.
/// </summary>
public interface IIntegrationEventOutbox
{
    void Enqueuear(string routingKey, object integrationEvent);
}
