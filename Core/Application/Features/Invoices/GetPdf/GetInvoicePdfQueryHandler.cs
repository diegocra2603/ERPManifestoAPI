using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.InvoicePdf;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.GetPdf;

public class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, ErrorOr<InvoicePdfResult>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IInvoicePdfService _pdfService;

    public GetInvoicePdfQueryHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IInvoicePdfService pdfService)
    {
        _invoiceRepository = invoiceRepository;
        _pdfService = pdfService;
    }

    public async Task<ErrorOr<InvoicePdfResult>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
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

        var pdfBytes = await _pdfService.GeneratePdfAsync(invoice);
        var prefix = invoice.InvoiceType == Domain.Entities.Accounting.Enums.InvoiceType.CreditNote ? "NC" : "Factura";
        var fileName = $"{prefix}-{invoice.InvoiceNumber}.pdf";

        return new InvoicePdfResult(pdfBytes, fileName);
    }
}
