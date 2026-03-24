using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.InvoicePdf;
using Domain.Entities.Accounting;
using Domain.Entities.FiscalDocuments;
using Domain.Primitives.Mediator;
using ErrorOr;
using FiscalDoc = Domain.Entities.FiscalDocuments.FiscalDocument;

namespace Application.Features.Invoices.GetPdf;

public class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, ErrorOr<InvoicePdfResult>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IAsyncRepository<FiscalDoc> _fiscalDocRepository;
    private readonly IInvoicePdfService _pdfService;

    public GetInvoicePdfQueryHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IAsyncRepository<FiscalDoc> fiscalDocRepository,
        IInvoicePdfService pdfService)
    {
        _invoiceRepository = invoiceRepository;
        _fiscalDocRepository = fiscalDocRepository;
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

        // Load FiscalDocument to get Serie Admin and Numero Admin
        string? serieAdmin = null;
        long? numeroAdmin = null;
        if (!string.IsNullOrEmpty(invoice.FiscalSerie) && !invoice.FiscalSerie.StartsWith("CONTINGENCIA"))
        {
            var fiscalDocs = await _fiscalDocRepository.GetAsync(
                fd => fd.Serie == invoice.FiscalSerie &&
                      fd.Preimpreso == invoice.FiscalNumero &&
                      fd.AuditField.IsActive);

            var fiscalDoc = fiscalDocs.FirstOrDefault();
            if (fiscalDoc is not null)
            {
                serieAdmin = fiscalDoc.SerieAdmin;
                numeroAdmin = fiscalDoc.NumeroAdmin;
            }
        }

        var pdfBytes = await _pdfService.GeneratePdfAsync(invoice, serieAdmin, numeroAdmin);
        var prefix = invoice.InvoiceType == Domain.Entities.Accounting.Enums.InvoiceType.CreditNote ? "NC" : "Factura";
        var fileName = $"{prefix}-{invoice.InvoiceNumber}.pdf";

        return new InvoicePdfResult(pdfBytes, fileName);
    }
}
