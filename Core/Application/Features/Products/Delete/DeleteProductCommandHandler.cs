using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.Delete;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ErrorOr<Unit>>
{
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IAsyncRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.Id);

        if (await _productRepository.FirstOrDefaultAsync(p => p.Id == productId) is not Product product)
        {
            return Error.NotFound("Product.NotFound", "Producto no encontrado.");
        }

        if (product.AuditField.IsDeleted)
        {
            return Error.Validation("Product.AlreadyDeleted", "El producto ya fue eliminado.");
        }

        product.MarkAsDeleted();

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
