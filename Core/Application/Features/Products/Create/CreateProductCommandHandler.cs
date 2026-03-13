using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Products.Create;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<ProductDto>>
{
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IAsyncRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _productRepository.ExistsAsync(p => p.Name == request.Name && p.AuditField.IsActive))
        {
            return Error.Validation("Product.NameAlreadyExists", "Ya existe un producto con ese nombre.");
        }

        var product = new Product(
            new ProductId(Guid.NewGuid()),
            request.Name,
            request.Description,
            AuditField.Create()
        );

        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id.Value,
            product.Name,
            product.Description,
            0,
            []
        );
    }
}
