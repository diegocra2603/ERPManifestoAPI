using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.InvoiceAccounting;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;
using ErrorOr;

namespace Services.InvoiceAccounting;

public sealed class InvoiceAccountingService : IInvoiceAccountingService
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;

    public InvoiceAccountingService(
        IAsyncRepository<JournalEntry> journalEntryRepository,
        IAsyncRepository<AccountingPeriod> periodRepository)
    {
        _journalEntryRepository = journalEntryRepository;
        _periodRepository = periodRepository;
    }

    public async Task<ErrorOr<JournalEntry>> CreateJournalEntryForEmissionAsync(
        Invoice invoice, CancellationToken ct = default)
    {
        var periodResult = await FindOpenPeriodAsync(invoice.Date);
        if (periodResult.IsError) return periodResult.Errors;

        var period = periodResult.Value;
        var entryNumber = await GetNextEntryNumberAsync();
        var entryId = new JournalEntryId(Guid.NewGuid());

        var description = invoice.InvoiceType == InvoiceType.Receivable
            ? $"Factura por cobrar {invoice.InvoiceNumber} - {invoice.Name}"
            : $"Factura por pagar {invoice.InvoiceNumber} - {invoice.Name}";

        var entry = new JournalEntry(
            entryId,
            entryNumber,
            invoice.Date,
            description,
            JournalEntryType.Diario,
            JournalEntryStatus.Aprobado,
            period.Id,
            invoice.CurrencyId,
            invoice.ExchangeRate,
            AuditField.Create());

        var lines = invoice.InvoiceType == InvoiceType.Receivable
            ? BuildReceivableLines(entryId, invoice)
            : BuildPayableLines(entryId, invoice);

        foreach (var line in lines)
            entry.AddLine(line);

        _journalEntryRepository.Add(entry);
        invoice.SetJournalEntry(entryId);

        return entry;
    }

    public async Task<ErrorOr<JournalEntry>> CreateReversalJournalEntryAsync(
        Invoice invoice, CancellationToken ct = default)
    {
        if (invoice.JournalEntryId is null)
            return Error.Validation("Invoice.NoJournalEntry", "La factura no tiene partida contable asociada.");

        // Load original journal entry
        var originalEntries = await _journalEntryRepository.GetAsync(
            predicate: e => e.Id == invoice.JournalEntryId && e.AuditField.IsActive,
            includes: [e => e.Lines],
            disableTracking: false);

        var original = originalEntries.FirstOrDefault();
        if (original is null)
            return Error.NotFound("JournalEntry.NotFound", "Partida contable original no encontrada.");

        // Void original entry
        original.Void();
        _journalEntryRepository.Update(original);

        // Create reversal entry
        var periodResult = await FindOpenPeriodAsync(DateTime.UtcNow);
        if (periodResult.IsError) return periodResult.Errors;

        var period = periodResult.Value;
        var entryNumber = await GetNextEntryNumberAsync();
        var reversalId = new JournalEntryId(Guid.NewGuid());

        var reversalEntry = new JournalEntry(
            reversalId,
            entryNumber,
            DateTime.UtcNow,
            $"Reversa por anulación - {invoice.InvoiceNumber}",
            JournalEntryType.Ajuste,
            JournalEntryStatus.Aprobado,
            period.Id,
            invoice.CurrencyId,
            invoice.ExchangeRate,
            AuditField.Create());

        // Reverse all lines: swap debit/credit
        var lineOrder = 1;
        foreach (var line in original.Lines.OrderBy(l => l.LineOrder))
        {
            reversalEntry.AddLine(new JournalEntryLine(
                new JournalEntryLineId(Guid.NewGuid()),
                reversalId,
                line.AccountId,
                $"Reversa: {line.Description}",
                line.Credit,
                line.Debit,
                Math.Round(line.Credit * invoice.ExchangeRate, 2),
                Math.Round(line.Debit * invoice.ExchangeRate, 2),
                lineOrder++));
        }

        _journalEntryRepository.Add(reversalEntry);

        return reversalEntry;
    }

    private List<JournalEntryLine> BuildReceivableLines(JournalEntryId entryId, Invoice invoice)
    {
        var rate = invoice.ExchangeRate;
        var lines = new List<JournalEntryLine>();

        // Debit: Cuentas por Cobrar (1.1.03) → Total
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.CuentasPorCobrar),
            $"CxC - {invoice.InvoiceNumber}",
            invoice.Total, 0,
            Math.Round(invoice.Total * rate, 2), 0,
            1));

        // Credit: Ventas de Servicios (4.1.02) → Subtotal
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.VentasDeServicios),
            $"Venta - {invoice.InvoiceNumber}",
            0, invoice.Subtotal,
            0, Math.Round(invoice.Subtotal * rate, 2),
            2));

        // Credit: IVA por Pagar (2.1.02) → TaxAmount
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.IvaPorPagar),
            $"IVA Débito Fiscal - {invoice.InvoiceNumber}",
            0, invoice.TaxAmount,
            0, Math.Round(invoice.TaxAmount * rate, 2),
            3));

        return lines;
    }

    private List<JournalEntryLine> BuildPayableLines(JournalEntryId entryId, Invoice invoice)
    {
        var rate = invoice.ExchangeRate;
        var lines = new List<JournalEntryLine>();

        // Debit: Compras de Mercancías (6.1.01) → Subtotal
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.ComprasDeMercancias),
            $"Compra - {invoice.InvoiceNumber}",
            invoice.Subtotal, 0,
            Math.Round(invoice.Subtotal * rate, 2), 0,
            1));

        // Debit: IVA por Cobrar (1.1.04) → TaxAmount
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.IvaPorCobrar),
            $"IVA Crédito Fiscal - {invoice.InvoiceNumber}",
            invoice.TaxAmount, 0,
            Math.Round(invoice.TaxAmount * rate, 2), 0,
            2));

        // Credit: Cuentas por Pagar (2.1.01) → Total
        lines.Add(new JournalEntryLine(
            new JournalEntryLineId(Guid.NewGuid()),
            entryId,
            new AccountCatalogId(AccountingSeedData.AccountIds.CuentasPorPagar),
            $"CxP - {invoice.InvoiceNumber}",
            0, invoice.Total,
            0, Math.Round(invoice.Total * rate, 2),
            3));

        return lines;
    }

    private async Task<ErrorOr<AccountingPeriod>> FindOpenPeriodAsync(DateTime date)
    {
        var period = await _periodRepository.FirstOrDefaultAsync(
            p => p.Status == PeriodStatus.Abierto
                 && p.StartDate <= date
                 && p.EndDate >= date
                 && p.AuditField.IsActive);

        if (period is null)
            return Error.Validation("Period.NotFound",
                $"No se encontró un período contable abierto para la fecha {date:dd/MM/yyyy}.");

        return period;
    }

    private async Task<int> GetNextEntryNumberAsync()
    {
        var entries = await _journalEntryRepository.GetAsync(e => e.AuditField.IsActive);
        return entries.Count + 1;
    }
}
