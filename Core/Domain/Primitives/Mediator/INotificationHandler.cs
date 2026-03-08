namespace Domain.Primitives.Mediator;

/// <summary>
/// Define un handler que procesa una notificación.
/// </summary>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
