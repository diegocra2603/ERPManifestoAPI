using Domain.Entities.Permissions;
using Domain.Primitives;
using Domain.ValueObjects;
namespace Domain.Entities.Roles;

public sealed class Role : AggregateRoot
{
    public Role() { }

    public Role(
        RoleId id,
        string name,
        string description,
        AuditField auditField
        )
    {
        Id = id;
        Name = name;
        Description = description;
        AuditField = auditField;
    }

    public RoleId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    // Relationships (ICollection para que EF pueda cargar la relación many-to-many)
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();
}
