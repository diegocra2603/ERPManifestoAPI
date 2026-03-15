using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Update;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<ProductDto>>
{
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IAsyncRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.Id);

        var products = await _productRepository.GetAsync(
            predicate: p => p.Id == productId,
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

        if (product.AuditField.IsDeleted)
        {
            return Error.NotFound("Product.NotFound", "El producto ya fue eliminado.");
        }

        var existingWithName = await _productRepository.FirstOrDefaultAsync(p => p.Name == request.Name && p.AuditField.IsActive);
        if (existingWithName is not null && existingWithName.Id != product.Id)
        {
            return Error.Validation("Product.NameAlreadyExists", "Ya existe un producto con ese nombre.");
        }

        product.Update(request.Name, request.Description);

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
