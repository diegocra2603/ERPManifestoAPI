namespace Domain.Primitives.Mediator;

/// <summary>
/// Envía un request al handler correspondiente.
/// </summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
