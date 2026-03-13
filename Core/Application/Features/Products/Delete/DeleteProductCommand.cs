using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<ErrorOr<Unit>>;
