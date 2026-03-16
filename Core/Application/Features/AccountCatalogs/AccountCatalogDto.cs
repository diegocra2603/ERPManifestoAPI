namespace Application.Features.AccountCatalogs;

public record AccountCatalogDto(
    Guid Id,
    string AccountCode,
    string Name,
    int AccountType,
    string AccountTypeName,
    int Nature,
    string NatureName,
    Guid? ParentId,
    string? ParentName,
    int Level,
    bool AcceptsMovements,
    IReadOnlyList<AccountCatalogDto> Children
);
