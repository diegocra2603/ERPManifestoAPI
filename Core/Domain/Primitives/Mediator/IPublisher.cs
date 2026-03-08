namespace Domain.Primitives.Mediator;

/// <summary>
/// Publica notificaciones a todos los handlers registrados.
/// </summary>
public interface IPublisher
{
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}
