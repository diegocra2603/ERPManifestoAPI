namespace Domain.Entities.Briefs;

public record BriefDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ClientName,
    string ContactName,
    DateTime Date,
    string? CompanyBackground,
    string? BrandingElementsToPreserve,
    string? CommunicationProblems,
    string? BrandPerception,
    DateTime? StartDate,
    DateTime? DeliveryDate,
    int? DurationMonths,
    decimal? Budget,
    IReadOnlyList<BriefItemDto> Items
);

public record BriefItemDto(
    Guid Id,
    BriefSectionType SectionType,
    string ItemName,
    bool IsSelected,
    string? Comments
);
