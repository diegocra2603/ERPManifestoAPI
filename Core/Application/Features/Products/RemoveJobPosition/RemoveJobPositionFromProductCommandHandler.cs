using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.RemoveJobPosition;

public class RemoveJobPositionFromProductCommandHandler : IRequestHandler<RemoveJobPositionFromProductCommand, ErrorOr<ProductDto>>
{
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IAsyncRepository<ProductJobPosition> _productJobPositionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveJobPositionFromProductCommandHandler(
        IAsyncRepository<Product> productRepository,
        IAsyncRepository<ProductJobPosition> productJobPositionRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productJobPositionRepository = productJobPositionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ProductDto>> Handle(RemoveJobPositionFromProductCommand request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);

        var products = await _productRepository.GetAsync(
            predicate: p => p.Id == productId && p.AuditField.IsActive,
            includes: new()
            {
                p => p.JobPositions
            },
            disableTracking: false);

        var product = products.FirstOrDefault();

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", "Producto no encontrado.");
        }

        var productJobPositionId = new ProductJobPositionId(request.ProductJobPositionId);
        var jobPositionToRemove = product.JobPositions.FirstOrDefault(jp => jp.Id == productJobPositionId);

        if (jobPositionToRemove is null)
        {
            return Error.NotFound("Product.JobPositionNotFound", "El puesto de trabajo no está asignado a este producto.");
        }

        product.RemoveJobPosition(productJobPositionId);
        _productJobPositionRepository.Delete(jobPositionToRemove);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
