using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Products.AddJobPosition;

public class AddJobPositionToProductCommandHandler : IRequestHandler<AddJobPositionToProductCommand, ErrorOr<ProductDto>>
{
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddJobPositionToProductCommandHandler(
        IAsyncRepository<Product> productRepository,
        IAsyncRepository<JobPosition> jobPositionRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _jobPositionRepository = jobPositionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ProductDto>> Handle(AddJobPositionToProductCommand request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);

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

        var jobPositionId = new JobPositionId(request.JobPositionId);

        if (await _jobPositionRepository.FirstOrDefaultAsync(jp => jp.Id == jobPositionId && jp.AuditField.IsActive) is not JobPosition jobPosition)
        {
            return Error.NotFound("JobPosition.NotFound", "Puesto de trabajo no encontrado.");
        }

        if (product.JobPositions.Any(jp => jp.JobPositionId == jobPositionId))
        {
            return Error.Validation("Product.JobPositionAlreadyExists", "Este puesto de trabajo ya está asignado al producto.");
        }

        var productJobPosition = new ProductJobPosition(
            new ProductJobPositionId(Guid.NewGuid()),
            productId,
            jobPositionId,
            request.Hours
        );

        product.AddJobPosition(productJobPosition);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id.Value,
            product.Name,
            product.Description,
            product.JobPositions.Sum(jp => jp.Hours * jobPosition.HourlyCost),
            product.JobPositions.Select(jp => new ProductJobPositionDto(
                jp.Id.Value,
                jp.JobPositionId.Value,
                jp.JobPositionId == jobPositionId ? jobPosition.Name : jp.JobPosition.Name,
                jp.Hours,
                jp.JobPositionId == jobPositionId ? jobPosition.HourlyCost : jp.JobPosition.HourlyCost,
                jp.Hours * (jp.JobPositionId == jobPositionId ? jobPosition.HourlyCost : jp.JobPosition.HourlyCost)
            )).ToList()
        );
    }
}
