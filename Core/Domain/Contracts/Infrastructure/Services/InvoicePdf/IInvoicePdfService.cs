using Domain.Entities.Accounting;

namespace Domain.Contracts.Infrastructure.Services.InvoicePdf;

public interface IInvoicePdfService
{
    Task<byte[]> GeneratePdfAsync(Invoice invoice, string? serieAdmin = null, long? numeroAdmin = null);
}
