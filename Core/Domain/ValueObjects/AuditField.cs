namespace Domain.ValueObjects;

public partial record AuditField
{
    public AuditField(DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt, bool isActive)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        IsActive = isActive;
    }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public bool IsActive { get; init; }
    public static AuditField Create() => new(DateTime.UtcNow, null, null, true);

    public static AuditField Create(DateTime createdAt) => new(createdAt, null, null, true);

    public AuditField Update() => this with { UpdatedAt = DateTime.UtcNow };

    public AuditField Update(DateTime updatedAt) => this with { UpdatedAt = updatedAt };

    public AuditField MarkDeleted() => this with { DeletedAt = DateTime.UtcNow, IsActive = false };

    public AuditField MarkDeleted(DateTime deletedAt) => this with { DeletedAt = deletedAt, IsActive = false };

    public bool IsDeleted => DeletedAt.HasValue;
}
