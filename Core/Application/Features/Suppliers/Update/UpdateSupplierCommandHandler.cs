using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.Update;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, ErrorOr<SupplierDto>>
{
    private readonly IAsyncRepository<Supplier> _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(
        IAsyncRepository<Supplier> supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.FirstOrDefaultAsync(s => s.Id == new SupplierId(request.Id) && s.AuditField.IsActive);
        if (supplier is null)
            return Error.NotFound("Supplier.NotFound", "Proveedor no encontrado.");

        if (await _supplierRepository.ExistsAsync(s => s.Nit == request.Nit && s.Id != new SupplierId(request.Id) && s.AuditField.IsActive))
            return Error.Validation("Supplier.NitAlreadyExists", "Ya existe otro proveedor con ese NIT.");

        supplier.Update(request.Nit, request.Name, request.Address, request.Phone, request.Email);
        _supplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SupplierDto(supplier.Id.Value, supplier.Nit, supplier.Name, supplier.Address, supplier.Phone, supplier.Email);
    }
}
