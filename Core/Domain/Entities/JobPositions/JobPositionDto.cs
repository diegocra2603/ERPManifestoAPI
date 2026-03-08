namespace Domain.Entities.JobPositions;

public record JobPositionDto(
    Guid Id,
    string Name,
    string Description,
    decimal HourlyCost
);
