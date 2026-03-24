using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Contracts.Infrastructure.Services.InvoiceAccounting;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Entities.FiscalDocuments;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Emit;

public class EmitInvoiceCommandHandler : IRequestHandler<EmitInvoiceCommand, ErrorOr<InvoiceDto>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IAsyncRepository<TaxConfiguration> _taxConfigRepository;
    private readonly IAsyncRepository<Domain.Entities.FiscalDocuments.FiscalDocument> _fiscalDocRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDocumentService _fiscalDocumentService;
    private readonly IInvoiceAccountingService _accountingService;

    public EmitInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IAsyncRepository<TaxConfiguration> taxConfigRepository,
        IAsyncRepository<Domain.Entities.FiscalDocuments.FiscalDocument> fiscalDocRepository,
        IUnitOfWork unitOfWork,
        IFiscalDocumentService fiscalDocumentService,
        IInvoiceAccountingService accountingService)
    {
        _invoiceRepository = invoiceRepository;
        _taxConfigRepository = taxConfigRepository;
        _fiscalDocRepository = fiscalDocRepository;
        _unitOfWork = unitOfWork;
        _fiscalDocumentService = fiscalDocumentService;
        _accountingService = accountingService;
    }

    public async Task<ErrorOr<InvoiceDto>> Handle(EmitInvoiceCommand request, CancellationToken cancellationToken)
    {
        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Items,
            e => e.Client!,
            e => e.Supplier!,
            e => e.Currency,
            e => e.OriginalInvoice!
        };

        // Load WITH tracking so we can update after Ainnova call
        var invoices = await _invoiceRepository.GetAsync(
            predicate: i => i.Id == new InvoiceId(request.Id) && i.AuditField.IsActive,
            includes: includes,
            disableTracking: false);

        var invoice = invoices.FirstOrDefault();

        if (invoice is null)
            return Error.NotFound("Invoice.NotFound", "Factura no encontrada.");

        if (invoice.Status != InvoiceStatus.Borrador)
            return Error.Validation("Invoice.NotDraft", "Solo se pueden emitir facturas en estado Borrador.");

        // Get IVA tax percentage from TaxConfiguration (SobreVenta)
        var ivaTax = await _taxConfigRepository.FirstOrDefaultAsync(
            t => t.TaxType == TaxType.SobreVenta && t.AuditField.IsActive);

        var taxPercentage = ivaTax?.Percentage ?? 12.00m;

        // Determine fiscal document type
        var documentType = invoice.InvoiceType switch
        {
            InvoiceType.Receivable => FiscalDocumentType.Factura,
            InvoiceType.CreditNote => FiscalDocumentType.NotaDeCredito,
            _ => FiscalDocumentType.FacturaEspecial
        };

        var currencyType = invoice.Currency?.Code == "USD"
            ? CurrencyType.Dolar
            : CurrencyType.Quetzal;

        // Build fiscal items and associated document data
        List<GenerateDocumentItemRequest> fiscalItems;
        string? docAsociadoSerie = null;
        string? docAsociadoPreimpreso = null;

        if (invoice.InvoiceType == InvoiceType.CreditNote && invoice.OriginalInvoice is not null)
        {
            docAsociadoSerie = invoice.OriginalInvoice.FiscalSerie;
            docAsociadoPreimpreso = invoice.OriginalInvoice.FiscalNumero;

            // For credit notes, use the original FiscalDocument's items to match SAT amounts exactly
            Domain.Entities.FiscalDocuments.FiscalDocument? originalFiscalDoc = null;
            if (!string.IsNullOrEmpty(docAsociadoSerie) && !string.IsNullOrEmpty(docAsociadoPreimpreso))
            {
                var fiscalDocs = await _fiscalDocRepository.GetAsync(
                    predicate: fd => fd.Serie == docAsociadoSerie &&
                                     fd.Preimpreso == docAsociadoPreimpreso &&
                                     fd.Status == FiscalDocumentStatus.Certified &&
                                     fd.AuditField.IsActive,
                    includeString: "Items");

                originalFiscalDoc = fiscalDocs.FirstOrDefault();
            }

            if (originalFiscalDoc is not null && originalFiscalDoc.Items.Any())
            {
                // Use the exact prices that were sent to SAT for the original invoice
                fiscalItems = originalFiscalDoc.Items.Select(fi => new GenerateDocumentItemRequest(
                    ProductCode: fi.ProductCode,
                    Description: fi.Description,
                    MeasureUnit: fi.MeasureUnit,
                    Quantity: fi.Quantity,
                    Price: fi.Price,
                    DiscountPercentage: fi.DiscountPercentage,
                    SaleType: fi.SaleType
                )).ToList();
            }
            else
            {
                // Fallback: build from invoice items with IVA-inclusive price
                fiscalItems = invoice.Items.OrderBy(i => i.LineOrder).Select(item => new GenerateDocumentItemRequest(
                    ProductCode: item.LineOrder.ToString(),
                    Description: item.Description,
                    MeasureUnit: 1,
                    Quantity: item.Quantity,
                    Price: Math.Round(item.Total / item.Quantity, 2),
                    DiscountPercentage: 0,
                    SaleType: SaleType.Servicios
                )).ToList();
            }
        }
        else
        {
            // Regular invoices: Price must include IVA (Ainnova/FEL expects IVA-inclusive prices)
            fiscalItems = invoice.Items.OrderBy(i => i.LineOrder).Select(item => new GenerateDocumentItemRequest(
                ProductCode: item.LineOrder.ToString(),
                Description: item.Description,
                MeasureUnit: 1,
                Quantity: item.Quantity,
                Price: Math.Round(item.Total / item.Quantity, 2),
                DiscountPercentage: 0,
                SaleType: SaleType.Servicios
            )).ToList();
        }

        // Send to Ainnova - pass exchange rate and tax percentage from accounting entities
        var fiscalRequest = new GenerateDocumentRequest(
            DocumentType: documentType,
            NitReceptor: invoice.Nit,
            NombreReceptor: invoice.Name,
            DireccionReceptor: invoice.Address,
            TipoVenta: SaleType.Servicios,
            DestinoVenta: DestinationType.Local,
            Moneda: currencyType,
            ExchangeRate: invoice.ExchangeRate,
            TaxPercentage: taxPercentage,
            Referencia: $"{invoice.InvoiceNumber}-{invoice.Id.Value.ToString("N")[..8]}",
            SerieAdmin: null,
            NumeroAdmin: null,
            DocAsociadoSerie: docAsociadoSerie,
            DocAsociadoPreimpreso: docAsociadoPreimpreso,
            Items: fiscalItems
        );

        var fiscalResult = await _fiscalDocumentService.GenerateDocumentAsync(fiscalRequest);

        if (fiscalResult.IsError)
            return fiscalResult.Errors;

        var fiscalDoc = fiscalResult.Value;

        // Set fiscal data and emit
        if (fiscalDoc.Status == FiscalDocumentStatus.Certified &&
            !string.IsNullOrEmpty(fiscalDoc.Serie))
        {
            invoice.SetFiscalData(fiscalDoc.Serie, fiscalDoc.Preimpreso, fiscalDoc.NumeroAutorizacion);
        }
        else if (fiscalDoc.Status == FiscalDocumentStatus.Contingency)
        {
            invoice.SetFiscalData(
                $"CONTINGENCIA-{fiscalDoc.NumeroAcceso}",
                fiscalDoc.NumeroAcceso?.ToString() ?? "0",
                "PENDIENTE - Documento en contingencia");
        }
        else
        {
            invoice.SetFiscalData(fiscalDoc.Serie, fiscalDoc.Preimpreso, fiscalDoc.NumeroAutorizacion);
        }

        invoice.Emit();

        // Create journal entry for the emitted invoice
        var journalResult = await _accountingService.CreateJournalEntryForEmissionAsync(invoice, cancellationToken);
        if (journalResult.IsError)
            return journalResult.Errors;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return InvoiceMapper.MapToDto(invoice);
    }
}
