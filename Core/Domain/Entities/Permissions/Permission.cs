using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Permissions;

public sealed class Permission : AggregateRoot
{
    public Permission() { }

    public Permission(
        PermissionId id,
        string name,
        string description,
        string code,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        Description = description;
        Code = code;
        AuditField = auditField;
    }

    public PermissionId Id { get; private set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Code { get; set; } = default!;
    public AuditField AuditField { get; set; } = default!;
}