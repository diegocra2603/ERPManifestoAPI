using Domain.Entities.Accounting;

namespace Application.Features.Invoices;

public static class InvoiceMapper
{
    public static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id.Value,
            (int)invoice.InvoiceType,
            invoice.InvoiceType.ToString(),
            (int)invoice.Status,
            invoice.Status.ToString(),
            invoice.InvoiceNumber,
            invoice.Date,
            invoice.DueDate,
            invoice.ClientId?.Value,
            invoice.Client?.Name,
            invoice.SupplierId?.Value,
            invoice.Supplier?.Name,
            invoice.Nit,
            invoice.Name,
            invoice.Address,
            invoice.CurrencyId.Value,
            invoice.Currency?.Code ?? "",
            invoice.ExchangeRate,
            invoice.Subtotal,
            invoice.TaxAmount,
            invoice.Total,
            invoice.Notes,
            invoice.JournalEntryId?.Value,
            invoice.FiscalSerie,
            invoice.FiscalNumero,
            invoice.FiscalAutorizacion,
            invoice.OriginalInvoiceId?.Value,
            invoice.OriginalInvoice?.InvoiceNumber,
            invoice.OriginalInvoice?.FiscalSerie,
            invoice.OriginalInvoice?.FiscalNumero,
            invoice.Items.Select(i => new InvoiceItemDto(
                i.Id.Value,
                i.Description,
                i.Quantity,
                i.UnitPrice,
                i.Subtotal,
                i.TaxAmount,
                i.Total,
                i.LineOrder
            )).OrderBy(i => i.LineOrder).ToList().AsReadOnly()
        );
    }
}
