using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Get;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductDto>>
{
    private readonly IAsyncRepository<Product> _productRepository;

    public GetProductByIdQueryHandler(IAsyncRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.Id);

        var products = await _productRepository.GetAsync(
            predicate: p => p.Id == productId && p.AuditField.IsActive,
            includes: new()
            {
                p => p.JobPositions
            });

        var product = products.FirstOrDefault();

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", "Producto no encontrado.");
        }

        return new ProductDto(
            product.Id.Value,
            product.Name,
            product.Description,
            product.TotalCost,
            product.JobPositions.Select(jp => new ProductJobPositionDto(
                jp.Id.Value,
                jp.JobPositionId.Value,
                jp.JobPosition.Name,
                jp.Hours,
                jp.JobPosition.HourlyCost,
                jp.Hours * jp.JobPosition.HourlyCost
            )).ToList()
        );
    }
}
