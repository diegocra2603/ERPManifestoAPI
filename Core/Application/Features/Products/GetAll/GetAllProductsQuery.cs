using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.GetAll;

public record GetAllProductsQuery : IRequest<ErrorOr<IReadOnlyList<ProductDto>>>;
