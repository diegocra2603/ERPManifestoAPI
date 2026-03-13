using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.AddJobPosition;

public record AddJobPositionToProductCommand(
    Guid ProductId,
    Guid JobPositionId,
    decimal Hours) : IRequest<ErrorOr<ProductDto>>;
