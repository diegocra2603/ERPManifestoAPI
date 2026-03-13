using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Create;

public record CreateProductCommand(
    string Name,
    string Description) : IRequest<ErrorOr<ProductDto>>;
