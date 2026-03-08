using System.Reflection;
using Domain.Primitives.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Mediator;

/// <summary>
/// Extensiones para registrar el mediator local y sus handlers en el contenedor de DI.
/// </summary>
public static class MediatorServiceExtensions
{
    /// <summary>
    /// Registra el mediator, handlers, behaviors y validators del assembly indicado.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Registrar ISender e IPublisher
        services.AddScoped<ISender, Mediator>();
        services.AddScoped<IPublisher, Mediator>();

        foreach (var assembly in assemblies)
        {
            RegisterHandlers(services, assembly);
            RegisterNotificationHandlers(services, assembly);
            RegisterPipelineBehaviors(services, assembly);
        }

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlers = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }

    private static void RegisterNotificationHandlers(IServiceCollection services, Assembly assembly)
    {
        var notificationHandlerType = typeof(INotificationHandler<>);

        var handlers = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == notificationHandlerType)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }

    private static void RegisterPipelineBehaviors(IServiceCollection services, Assembly assembly)
    {
        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);

        var behaviors = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == pipelineBehaviorType))
            .ToList();

        foreach (var behavior in behaviors)
        {
            // Open generic behaviors (como ValidationBehavior<,>) se registran como open generic
            if (behavior.IsGenericTypeDefinition)
            {
                services.AddScoped(pipelineBehaviorType, behavior);
            }
            else
            {
                var interfaceType = behavior.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == pipelineBehaviorType);
                services.AddScoped(interfaceType, behavior);
            }
        }
    }
}
