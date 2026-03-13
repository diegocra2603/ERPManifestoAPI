using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Get;

public record GetProductByIdQuery(Guid Id) : IRequest<ErrorOr<ProductDto>>;
