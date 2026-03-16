using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.Delete;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<Supplier> _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSupplierCommandHandler(
        IAsyncRepository<Supplier> supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.FirstOrDefaultAsync(s => s.Id == new SupplierId(request.Id) && s.AuditField.IsActive);
        if (supplier is null)
            return Error.NotFound("Supplier.NotFound", "Proveedor no encontrado.");

        supplier.MarkAsDeleted();
        _supplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
