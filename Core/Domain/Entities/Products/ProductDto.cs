namespace Domain.Entities.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal TotalCost,
    IReadOnlyList<ProductJobPositionDto> JobPositions
);

public record ProductJobPositionDto(
    Guid Id,
    Guid JobPositionId,
    string JobPositionName,
    decimal Hours,
    decimal HourlyCost,
    decimal Subtotal
);
