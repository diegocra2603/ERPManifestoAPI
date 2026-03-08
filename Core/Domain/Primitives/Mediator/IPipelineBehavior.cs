namespace Domain.Primitives.Mediator;

/// <summary>
/// Delegado que representa el siguiente paso en el pipeline.
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Define un comportamiento de pipeline que envuelve la ejecución del handler.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
