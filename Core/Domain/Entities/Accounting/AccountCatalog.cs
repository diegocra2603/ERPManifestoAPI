using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class AccountCatalog : AggregateRoot
{
    private readonly List<AccountCatalog> _children = new();

    private AccountCatalog() { }

    public AccountCatalog(
        AccountCatalogId id,
        string accountCode,
        string name,
        AccountType accountType,
        AccountNature nature,
        AccountCatalogId? parentId,
        int level,
        bool acceptsMovements,
        AuditField auditField)
    {
        Id = id;
        AccountCode = accountCode;
        Name = name;
        AccountType = accountType;
        Nature = nature;
        ParentId = parentId;
        Level = level;
        AcceptsMovements = acceptsMovements;
        AuditField = auditField;
    }

    public AccountCatalogId Id { get; private set; } = default!;
    public string AccountCode { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public AccountType AccountType { get; private set; }
    public AccountNature Nature { get; private set; }
    public AccountCatalogId? ParentId { get; private set; }
    public int Level { get; private set; }
    public bool AcceptsMovements { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public AccountCatalog? Parent { get; private set; }
    public IReadOnlyCollection<AccountCatalog> Children => _children.AsReadOnly();

    public void Update(string accountCode, string name, AccountType accountType, AccountNature nature,
        AccountCatalogId? parentId, int level, bool acceptsMovements)
    {
        AccountCode = accountCode;
        Name = name;
        AccountType = accountType;
        Nature = nature;
        ParentId = parentId;
        Level = level;
        AcceptsMovements = acceptsMovements;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
