using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDataValidator;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Suppliers.Create;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, ErrorOr<SupplierDto>>
{
    private readonly IAsyncRepository<Supplier> _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDataValidatorService _fiscalDataValidatorService;

    public CreateSupplierCommandHandler(
        IAsyncRepository<Supplier> supplierRepository,
        IUnitOfWork unitOfWork,
        IFiscalDataValidatorService fiscalDataValidatorService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _fiscalDataValidatorService = fiscalDataValidatorService;
    }

    public async Task<ErrorOr<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var fiscalResult = await _fiscalDataValidatorService.ValidateFiscalDataAsync(request.Nit);
        if (fiscalResult.IsError)
            return fiscalResult.Errors;

        if (await _supplierRepository.ExistsAsync(s => s.Nit == request.Nit && s.AuditField.IsActive))
        {
            return Error.Validation("Supplier.NitAlreadyExists", "Ya existe un proveedor con ese NIT.");
        }

        var supplier = new Supplier(
            new SupplierId(Guid.NewGuid()),
            request.Nit,
            fiscalResult.Value.FiscalName,
            request.Address,
            request.Phone,
            request.Email,
            AuditField.Create()
        );

        _supplierRepository.Add(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SupplierDto(
            supplier.Id.Value,
            supplier.Nit,
            supplier.Name,
            supplier.Address,
            supplier.Phone,
            supplier.Email
        );
    }
}
