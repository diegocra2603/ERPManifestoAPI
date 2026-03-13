using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.SystemSettings;

public sealed class SystemSetting : AggregateRoot
{
    private SystemSetting() { }

    public SystemSetting(
        SystemSettingId id,
        string key,
        string value,
        string description,
        AuditField auditField)
    {
        Id = id;
        Key = key;
        Value = value;
        Description = description;
        AuditField = auditField;
    }

    public SystemSettingId Id { get; private set; } = default!;
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    public void UpdateValue(string value)
    {
        Value = value;
        AuditField = AuditField.Update();
    }
}
