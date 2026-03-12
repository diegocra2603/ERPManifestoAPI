using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.FiscalData;

public sealed class FiscalDataEntry : AggregateRoot
{
    private FiscalDataEntry() { }

    public FiscalDataEntry(
        FiscalDataEntryId id,
        string fiscalCode,
        string fiscalName,
        AuditField auditField)
    {
        Id = id;
        FiscalCode = fiscalCode;
        FiscalName = fiscalName;
        AuditField = auditField;
    }

    public FiscalDataEntryId Id { get; private set; } = default!;
    public string FiscalCode { get; private set; } = default!;
    public string FiscalName { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    public void Update(string fiscalName)
    {
        FiscalName = fiscalName;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
