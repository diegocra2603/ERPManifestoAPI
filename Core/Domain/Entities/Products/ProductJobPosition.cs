using Domain.Entities.JobPositions;
using Domain.Primitives;

namespace Domain.Entities.Products;

public sealed class ProductJobPosition : Entity
{
    private ProductJobPosition() { }

    public ProductJobPosition(
        ProductJobPositionId id,
        ProductId productId,
        JobPositionId jobPositionId,
        decimal hours)
    {
        Id = id;
        ProductId = productId;
        JobPositionId = jobPositionId;
        Hours = hours;
    }

    public ProductJobPositionId Id { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public JobPositionId JobPositionId { get; private set; } = default!;
    public decimal Hours { get; private set; }

    // Navigation
    public Product Product { get; private set; } = default!;
    public JobPosition JobPosition { get; private set; } = default!;

    public void UpdateHours(decimal hours)
    {
        Hours = hours;
    }
}
