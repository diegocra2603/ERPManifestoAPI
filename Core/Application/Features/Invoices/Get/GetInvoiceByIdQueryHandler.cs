using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Get;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, ErrorOr<InvoiceDto>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IAsyncRepository<Invoice> invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ErrorOr<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoiceId = new InvoiceId(request.Id);

        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Client!,
            e => e.Supplier!,
            e => e.Currency,
            e => e.Items,
            e => e.OriginalInvoice!
        };

        var invoices = await _invoiceRepository.GetAsync(
            predicate: e => e.Id == invoiceId && e.AuditField.IsActive,
            includes: includes);

        var invoice = invoices.FirstOrDefault();

        if (invoice is null)
            return Error.NotFound("Invoice.NotFound", "Factura no encontrada.");

        return InvoiceMapper.MapToDto(invoice);
    }
}
