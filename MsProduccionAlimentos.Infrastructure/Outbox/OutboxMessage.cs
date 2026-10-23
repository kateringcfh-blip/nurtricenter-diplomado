namespace MsProduccionAlimentos.Infrastructure.Outbox;

/// <summary>
/// Fila de la tabla OutboxMessages: implementa el patrón Transactional Outbox.
/// Se inserta en la MISMA transacción que el cambio de negocio (ver EfOutboxWriter),
/// y un proceso aparte (OutboxPublisherBackgroundService) la publica a RabbitMQ y la
/// marca como procesada. Así se evita el problema de "doble escritura": si el proceso
/// cae justo después de guardar en la base, el evento no se pierde, solo se reintenta.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OcurridoEn { get; set; }
    public DateTime? ProcesadoEn { get; set; }
}
