using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Void;

public class VoidInvoiceCommandHandler : IRequestHandler<VoidInvoiceCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDocumentService _fiscalDocumentService;

    public VoidInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork,
        IFiscalDocumentService fiscalDocumentService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _fiscalDocumentService = fiscalDocumentService;
    }

    public async Task<ErrorOr<bool>> Handle(VoidInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            i => i.Id == new InvoiceId(request.Id) && i.AuditField.IsActive);

        if (invoice is null)
            return Error.NotFound("Invoice.NotFound", "Factura no encontrada.");

        if (invoice.Status == InvoiceStatus.Anulada)
            return Error.Validation("Invoice.AlreadyVoided", "La factura ya se encuentra anulada.");

        if (invoice.Status != InvoiceStatus.Emitida)
            return Error.Validation("Invoice.NotEmitted", "Solo se pueden anular facturas emitidas.");

        // If the invoice has fiscal data (was certified), void it in Ainnova
        if (!string.IsNullOrEmpty(invoice.FiscalSerie) &&
            !invoice.FiscalSerie.StartsWith("CONTINGENCIA"))
        {
            var now = DateTime.UtcNow;
            var voidResult = await _fiscalDocumentService.VoidDocumentAsync(new VoidDocumentRequest(
                Serie: invoice.FiscalSerie,
                Preimpreso: invoice.FiscalNumero!,
                NitComprador: invoice.Nit,
                FechaAnulacion: now.ToString("yyyyMMdd"),
                MotivoAnulacion: "Anulacion solicitada por el emisor"
            ));

            if (voidResult.IsError)
                return voidResult.Errors;
        }

        invoice.Void();
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
