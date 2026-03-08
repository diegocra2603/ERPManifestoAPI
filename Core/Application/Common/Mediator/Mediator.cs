using Domain.Primitives.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Mediator;

/// <summary>
/// Implementación local del mediator pattern.
/// Resuelve handlers y ejecuta pipeline behaviors.
/// </summary>
public sealed class Mediator : ISender, IPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Construir el delegado final que invoca al handler
        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var method = handlerType.GetMethod("Handle")!;
            return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        };

        // Obtener pipeline behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType).Reverse().ToList();

        // Construir el pipeline encadenando behaviors
        var pipeline = behaviors.Aggregate(handlerDelegate, (next, behavior) =>
        {
            var currentNext = next;
            return () =>
            {
                var method = behaviorType.GetMethod("Handle")!;
                return (Task<TResponse>)method.Invoke(behavior, [request, currentNext, cancellationToken])!;
            };
        });

        return pipeline();
    }

    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle")!;
            await (Task)method.Invoke(handler, [notification, cancellationToken])!;
        }
    }
}
