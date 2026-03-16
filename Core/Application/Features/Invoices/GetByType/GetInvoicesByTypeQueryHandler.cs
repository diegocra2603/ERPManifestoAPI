using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.GetByType;

public class GetInvoicesByTypeQueryHandler : IRequestHandler<GetInvoicesByTypeQuery, ErrorOr<IReadOnlyList<InvoiceDto>>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;

    public GetInvoicesByTypeQueryHandler(IAsyncRepository<Invoice> invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<InvoiceDto>>> Handle(GetInvoicesByTypeQuery request, CancellationToken cancellationToken)
    {
        var invoiceType = (InvoiceType)request.InvoiceType;

        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Client!,
            e => e.Supplier!,
            e => e.Currency,
            e => e.Items
        };

        var invoices = await _invoiceRepository.GetAsync(
            predicate: e => e.InvoiceType == invoiceType && e.AuditField.IsActive,
            includes: includes);

        return invoices
            .Select(InvoiceMapper.MapToDto)
            .ToList()
            .AsReadOnly();
    }
}
