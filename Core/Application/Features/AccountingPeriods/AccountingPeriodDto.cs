namespace Application.Features.AccountingPeriods;

public record AccountingPeriodDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    int Status,
    string StatusName
);
