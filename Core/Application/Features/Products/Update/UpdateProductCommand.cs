using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description) : IRequest<ErrorOr<ProductDto>>;
