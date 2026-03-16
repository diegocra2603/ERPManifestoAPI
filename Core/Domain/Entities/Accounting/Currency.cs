using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class Currency : AggregateRoot
{
    private Currency() { }

    public Currency(
        CurrencyId id,
        string code,
        string name,
        string symbol,
        bool isFunctional,
        int decimalPlaces,
        AuditField auditField)
    {
        Id = id;
        Code = code;
        Name = name;
        Symbol = symbol;
        IsFunctional = isFunctional;
        DecimalPlaces = decimalPlaces;
        AuditField = auditField;
    }

    public CurrencyId Id { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Symbol { get; private set; } = default!;
    public bool IsFunctional { get; private set; }
    public int DecimalPlaces { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    public void Update(string code, string name, string symbol, bool isFunctional, int decimalPlaces)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        IsFunctional = isFunctional;
        DecimalPlaces = decimalPlaces;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
