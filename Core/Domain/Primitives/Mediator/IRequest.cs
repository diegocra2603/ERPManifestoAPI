namespace Domain.Primitives.Mediator;

/// <summary>
/// Marca un objeto como una solicitud (command/query) que devuelve un resultado.
/// </summary>
public interface IRequest<out TResponse>;
