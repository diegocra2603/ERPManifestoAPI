using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.GetAll;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ErrorOr<IReadOnlyList<ProductDto>>>
{
    private readonly IAsyncRepository<Product> _productRepository;

    public GetAllProductsQueryHandler(IAsyncRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAsync(
            predicate: p => p.AuditField.IsActive,
            includeString: "JobPositions.JobPosition");

        return products
            .Select(p => new ProductDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.TotalCost,
                p.JobPositions.Select(jp => new ProductJobPositionDto(
                    jp.Id.Value,
                    jp.JobPositionId.Value,
                    jp.JobPosition.Name,
                    jp.Hours,
                    jp.JobPosition.HourlyCost,
                    jp.Hours * jp.JobPosition.HourlyCost
                )).ToList()
            ))
            .ToList()
            .AsReadOnly();
    }
}
