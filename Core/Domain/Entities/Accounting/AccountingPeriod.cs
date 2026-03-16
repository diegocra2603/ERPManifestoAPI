using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class AccountingPeriod : AggregateRoot
{
    private AccountingPeriod() { }

    public AccountingPeriod(
        AccountingPeriodId id,
        string name,
        DateTime startDate,
        DateTime endDate,
        PeriodStatus status,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        AuditField = auditField;
    }

    public AccountingPeriodId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PeriodStatus Status { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    public void Update(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        AuditField = AuditField.Update();
    }

    public void Close()
    {
        Status = PeriodStatus.Cerrado;
        AuditField = AuditField.Update();
    }

    public void Reopen()
    {
        Status = PeriodStatus.Abierto;
        AuditField = AuditField.Update();
    }

    public bool IsOpen => Status == PeriodStatus.Abierto;

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
