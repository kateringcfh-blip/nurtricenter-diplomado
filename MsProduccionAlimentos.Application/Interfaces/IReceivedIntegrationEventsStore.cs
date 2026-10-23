namespace MsProduccionAlimentos.Application.Interfaces;

/// <summary>
/// Registro en memoria de los últimos eventos de integración recibidos por RabbitMQ.
/// Existe únicamente para poder demostrar/verificar el flujo (ej. vía un endpoint GET)
/// sin depender solo de los logs de consola.
/// </summary>
public interface IReceivedIntegrationEventsStore
{
    void Registrar(string tipoEvento, string routingKey, object payload);
    IReadOnlyList<ReceivedIntegrationEvent> ObtenerUltimos(int cantidad = 20);
}

public sealed record ReceivedIntegrationEvent(
    string TipoEvento,
    string RoutingKey,
    DateTime RecibidoEn,
    object Payload);
