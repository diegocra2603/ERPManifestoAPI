using Domain.Entities.Accounting.Enums;
using Domain.Entities.FiscalDocuments;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class TaxTransaction : AggregateRoot
{
    private TaxTransaction() { }

    public TaxTransaction(
        TaxTransactionId id,
        TaxConfigurationId taxConfigurationId,
        JournalEntryId? journalEntryId,
        FiscalDocumentId? fiscalDocumentId,
        CurrencyId currencyId,
        decimal exchangeRate,
        decimal taxableBase,
        decimal taxAmount,
        decimal taxableBaseFunctional,
        decimal taxAmountFunctional,
        DateTime date,
        TransactionType transactionType,
        AuditField auditField)
    {
        Id = id;
        TaxConfigurationId = taxConfigurationId;
        JournalEntryId = journalEntryId;
        FiscalDocumentId = fiscalDocumentId;
        CurrencyId = currencyId;
        ExchangeRate = exchangeRate;
        TaxableBase = taxableBase;
        TaxAmount = taxAmount;
        TaxableBaseFunctional = taxableBaseFunctional;
        TaxAmountFunctional = taxAmountFunctional;
        Date = date;
        TransactionType = transactionType;
        AuditField = auditField;
    }

    public TaxTransactionId Id { get; private set; } = default!;
    public TaxConfigurationId TaxConfigurationId { get; private set; } = default!;
    public JournalEntryId? JournalEntryId { get; private set; }
    public FiscalDocumentId? FiscalDocumentId { get; private set; }
    public CurrencyId CurrencyId { get; private set; } = default!;
    public decimal ExchangeRate { get; private set; }
    public decimal TaxableBase { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TaxableBaseFunctional { get; private set; }
    public decimal TaxAmountFunctional { get; private set; }
    public DateTime Date { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public TaxConfiguration TaxConfiguration { get; private set; } = default!;
    public JournalEntry? JournalEntry { get; private set; }
    public FiscalDocument? FiscalDocument { get; private set; }
    public Currency Currency { get; private set; } = default!;

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
