namespace Domain.Primitives.Mediator;

/// <summary>
/// Define un handler que procesa un request y devuelve una respuesta.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
