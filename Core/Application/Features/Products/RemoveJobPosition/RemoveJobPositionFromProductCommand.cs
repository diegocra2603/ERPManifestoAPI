using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.RemoveJobPosition;

public record RemoveJobPositionFromProductCommand(
    Guid ProductId,
    Guid ProductJobPositionId) : IRequest<ErrorOr<ProductDto>>;
