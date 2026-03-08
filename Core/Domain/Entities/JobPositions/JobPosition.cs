using Domain.Entities.Users;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.JobPositions;

public sealed class JobPosition : AggregateRoot
{
    private JobPosition() { }

    public JobPosition(
        JobPositionId id,
        string name,
        string description,
        decimal hourlyCost,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        Description = description;
        HourlyCost = hourlyCost;
        AuditField = auditField;
    }

    public JobPositionId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal HourlyCost { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Relationships
    public ICollection<User> Users { get; private set; } = new List<User>();

    public void Update(string name, string description, decimal hourlyCost)
    {
        Name = name;
        Description = description;
        HourlyCost = hourlyCost;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
