using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Entities.FiscalDocuments;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.UploadContingency;

public class UploadContingencyCommandHandler : IRequestHandler<UploadContingencyCommand, ErrorOr<UploadContingencyResult>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDocumentService _fiscalDocumentService;

    public UploadContingencyCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork,
        IFiscalDocumentService fiscalDocumentService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _fiscalDocumentService = fiscalDocumentService;
    }

    public async Task<ErrorOr<UploadContingencyResult>> Handle(UploadContingencyCommand request, CancellationToken cancellationToken)
    {
        // First, upload contingency fiscal documents to Ainnova
        var uploadResult = await _fiscalDocumentService.UploadContingencyDocumentsAsync();

        if (uploadResult.IsError)
            return uploadResult.Errors;

        var certifiedDocs = uploadResult.Value;
        var details = new List<string>();
        var updated = 0;

        // For each certified document, find the matching invoice by Referencia and update fiscal data
        foreach (var doc in certifiedDocs)
        {
            var invoices = await _invoiceRepository.GetAsync(
                i => i.InvoiceNumber == doc.Referencia && i.AuditField.IsActive);

            var invoice = invoices.FirstOrDefault();
            if (invoice is null)
            {
                details.Add($"{doc.Referencia}: Certificada en Ainnova pero no se encontró la factura local.");
                continue;
            }

            // Update invoice with fiscal data from the now-certified document
            invoice.SetFiscalData(doc.Serie, doc.Preimpreso, doc.NumeroAutorizacion);
            _invoiceRepository.Update(invoice);
            updated++;
            details.Add($"{doc.Referencia}: Serie={doc.Serie}, DTE={doc.Preimpreso}, Autorización={doc.NumeroAutorizacion}");
        }

        if (updated > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadContingencyResult(
            Uploaded: certifiedDocs.Count,
            Failed: certifiedDocs.Count - updated,
            Details: details
        );
    }
}
