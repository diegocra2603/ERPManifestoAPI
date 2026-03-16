using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class TaxConfiguration : AggregateRoot
{
    private TaxConfiguration() { }

    public TaxConfiguration(
        TaxConfigurationId id,
        string name,
        decimal percentage,
        TaxType taxType,
        AccountCatalogId debitAccountId,
        AccountCatalogId creditAccountId,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        Percentage = percentage;
        TaxType = taxType;
        DebitAccountId = debitAccountId;
        CreditAccountId = creditAccountId;
        AuditField = auditField;
    }

    public TaxConfigurationId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public decimal Percentage { get; private set; }
    public TaxType TaxType { get; private set; }
    public AccountCatalogId DebitAccountId { get; private set; } = default!;
    public AccountCatalogId CreditAccountId { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public AccountCatalog DebitAccount { get; private set; } = default!;
    public AccountCatalog CreditAccount { get; private set; } = default!;

    public void Update(string name, decimal percentage, TaxType taxType,
        AccountCatalogId debitAccountId, AccountCatalogId creditAccountId)
    {
        Name = name;
        Percentage = percentage;
        TaxType = taxType;
        DebitAccountId = debitAccountId;
        CreditAccountId = creditAccountId;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
