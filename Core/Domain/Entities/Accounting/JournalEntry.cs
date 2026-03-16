using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class JournalEntry : AggregateRoot
{
    private readonly List<JournalEntryLine> _lines = new();

    private JournalEntry() { }

    public JournalEntry(
        JournalEntryId id,
        int entryNumber,
        DateTime date,
        string description,
        JournalEntryType entryType,
        JournalEntryStatus status,
        AccountingPeriodId periodId,
        CurrencyId currencyId,
        decimal exchangeRate,
        AuditField auditField)
    {
        Id = id;
        EntryNumber = entryNumber;
        Date = date;
        Description = description;
        EntryType = entryType;
        Status = status;
        PeriodId = periodId;
        CurrencyId = currencyId;
        ExchangeRate = exchangeRate;
        AuditField = auditField;
    }

    public JournalEntryId Id { get; private set; } = default!;
    public int EntryNumber { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = default!;
    public JournalEntryType EntryType { get; private set; }
    public JournalEntryStatus Status { get; private set; }
    public AccountingPeriodId PeriodId { get; private set; } = default!;
    public CurrencyId CurrencyId { get; private set; } = default!;
    public decimal ExchangeRate { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public AccountingPeriod Period { get; private set; } = default!;
    public Currency Currency { get; private set; } = default!;
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.AsReadOnly();

    // Calculated
    public decimal TotalDebit => _lines.Sum(l => l.Debit);
    public decimal TotalCredit => _lines.Sum(l => l.Credit);
    public decimal TotalDebitFunctional => _lines.Sum(l => l.DebitFunctional);
    public decimal TotalCreditFunctional => _lines.Sum(l => l.CreditFunctional);
    public bool IsBalanced => TotalDebit == TotalCredit;

    public void AddLine(JournalEntryLine line)
    {
        _lines.Add(line);
        AuditField = AuditField.Update();
    }

    public void RemoveLine(JournalEntryLineId lineId)
    {
        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is not null)
        {
            _lines.Remove(line);
            AuditField = AuditField.Update();
        }
    }

    public void ClearLines()
    {
        _lines.Clear();
        AuditField = AuditField.Update();
    }

    public void Update(DateTime date, string description, JournalEntryType entryType,
        CurrencyId currencyId, decimal exchangeRate)
    {
        Date = date;
        Description = description;
        EntryType = entryType;
        CurrencyId = currencyId;
        ExchangeRate = exchangeRate;
        AuditField = AuditField.Update();
    }

    public void Approve()
    {
        Status = JournalEntryStatus.Aprobado;
        AuditField = AuditField.Update();
    }

    public void Void()
    {
        Status = JournalEntryStatus.Anulado;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
