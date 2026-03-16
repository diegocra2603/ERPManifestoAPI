using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.GetAll;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, ErrorOr<IReadOnlyList<SupplierDto>>>
{
    private readonly IAsyncRepository<Supplier> _supplierRepository;

    public GetAllSuppliersQueryHandler(IAsyncRepository<Supplier> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<SupplierDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAsync(s => s.AuditField.IsActive);

        return suppliers
            .Select(s => new SupplierDto(s.Id.Value, s.Nit, s.Name, s.Address, s.Phone, s.Email))
            .ToList()
            .AsReadOnly();
    }
}
