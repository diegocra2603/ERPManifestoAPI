using Domain.Primitives;

namespace Domain.Entities.Accounting;

public sealed class JournalEntryLine : Entity
{
    private JournalEntryLine() { }

    public JournalEntryLine(
        JournalEntryLineId id,
        JournalEntryId journalEntryId,
        AccountCatalogId accountId,
        string description,
        decimal debit,
        decimal credit,
        decimal debitFunctional,
        decimal creditFunctional,
        int lineOrder)
    {
        Id = id;
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Description = description;
        Debit = debit;
        Credit = credit;
        DebitFunctional = debitFunctional;
        CreditFunctional = creditFunctional;
        LineOrder = lineOrder;
    }

    public JournalEntryLineId Id { get; private set; } = default!;
    public JournalEntryId JournalEntryId { get; private set; } = default!;
    public AccountCatalogId AccountId { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public decimal DebitFunctional { get; private set; }
    public decimal CreditFunctional { get; private set; }
    public int LineOrder { get; private set; }

    // Navigation
    public JournalEntry JournalEntry { get; private set; } = default!;
    public AccountCatalog Account { get; private set; } = default!;

    public void Update(AccountCatalogId accountId, string description,
        decimal debit, decimal credit, decimal debitFunctional, decimal creditFunctional, int lineOrder)
    {
        AccountId = accountId;
        Description = description;
        Debit = debit;
        Credit = credit;
        DebitFunctional = debitFunctional;
        CreditFunctional = creditFunctional;
        LineOrder = lineOrder;
    }
}
