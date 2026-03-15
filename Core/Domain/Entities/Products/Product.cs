using Domain.Entities.JobPositions;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Products;

public sealed class Product : AggregateRoot
{
    private readonly List<ProductJobPosition> _jobPositions = new();
    private Product() { }

    public Product(
        ProductId id,
        string name,
        string description,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        Description = description;
        AuditField = auditField;
    }

    public ProductId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    public IReadOnlyCollection<ProductJobPosition> JobPositions => _jobPositions.AsReadOnly();
    /// <summary>
    /// Costo total calculado: suma de (horas × costo por hora) de cada puesto asignado.
    /// </summary>
    public decimal TotalCost => _jobPositions.Sum(pjp => pjp.Hours * pjp.JobPosition.HourlyCost);

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        AuditField = AuditField.Update();
    }

    public void AddJobPosition(ProductJobPosition jobPosition)
    {
        _jobPositions.Add(jobPosition);
        AuditField = AuditField.Update();
    }

    public void RemoveJobPosition(ProductJobPositionId jobPositionId)
    {
        var item = _jobPositions.FirstOrDefault(jp => jp.Id == jobPositionId);
        if (item is not null)
        {
            _jobPositions.Remove(item);
            AuditField = AuditField.Update();
        }
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
