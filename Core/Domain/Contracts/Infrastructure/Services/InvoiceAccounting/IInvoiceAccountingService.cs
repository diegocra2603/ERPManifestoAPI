using Domain.Entities.Accounting;
using ErrorOr;

namespace Domain.Contracts.Infrastructure.Services.InvoiceAccounting;

public interface IInvoiceAccountingService
{
    /// <summary>
    /// Creates a journal entry for an emitted invoice and links it to the invoice.
    /// Receivable: Debit CxC, Credit Ventas + IVA por Pagar
    /// Payable: Debit Compras + IVA por Cobrar, Credit CxP
    /// </summary>
    Task<ErrorOr<JournalEntry>> CreateJournalEntryForEmissionAsync(Invoice invoice, CancellationToken ct = default);

    /// <summary>
    /// Creates a reversal journal entry for a voided invoice and voids the original.
    /// </summary>
    Task<ErrorOr<JournalEntry>> CreateReversalJournalEntryAsync(Invoice invoice, CancellationToken ct = default);
}
