using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class ExchangeRate : AggregateRoot
{
    private ExchangeRate() { }

    public ExchangeRate(
        ExchangeRateId id,
        CurrencyId currencyId,
        DateTime date,
        decimal buyRate,
        decimal sellRate,
        string source,
        AuditField auditField)
    {
        Id = id;
        CurrencyId = currencyId;
        Date = date;
        BuyRate = buyRate;
        SellRate = sellRate;
        Source = source;
        AuditField = auditField;
    }

    public ExchangeRateId Id { get; private set; } = default!;
    public CurrencyId CurrencyId { get; private set; } = default!;
    public DateTime Date { get; private set; }
    public decimal BuyRate { get; private set; }
    public decimal SellRate { get; private set; }
    public string Source { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public Currency Currency { get; private set; } = default!;

    public void Update(decimal buyRate, decimal sellRate, string source)
    {
        BuyRate = buyRate;
        SellRate = sellRate;
        Source = source;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
