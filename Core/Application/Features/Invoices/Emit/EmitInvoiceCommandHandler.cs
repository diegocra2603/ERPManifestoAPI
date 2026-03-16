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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDocumentService _fiscalDocumentService;
    private readonly IInvoiceAccountingService _accountingService;

    public EmitInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IAsyncRepository<TaxConfiguration> taxConfigRepository,
        IUnitOfWork unitOfWork,
        IFiscalDocumentService fiscalDocumentService,
        IInvoiceAccountingService accountingService)
    {
        _invoiceRepository = invoiceRepository;
        _taxConfigRepository = taxConfigRepository;
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
            e => e.Currency
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
        var documentType = invoice.InvoiceType == InvoiceType.Receivable
            ? FiscalDocumentType.Factura
            : FiscalDocumentType.FacturaEspecial;

        var currencyType = invoice.Currency?.Code == "USD"
            ? CurrencyType.Dolar
            : CurrencyType.Quetzal;

        // Build fiscal items
        var fiscalItems = invoice.Items.OrderBy(i => i.LineOrder).Select(item => new GenerateDocumentItemRequest(
            ProductCode: item.LineOrder.ToString(),
            Description: item.Description,
            MeasureUnit: 1,
            Quantity: item.Quantity,
            Price: item.UnitPrice,
            DiscountPercentage: 0,
            SaleType: SaleType.Servicios
        )).ToList();

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
            Referencia: invoice.InvoiceNumber,
            SerieAdmin: null,
            NumeroAdmin: null,
            DocAsociadoSerie: null,
            DocAsociadoPreimpreso: null,
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
