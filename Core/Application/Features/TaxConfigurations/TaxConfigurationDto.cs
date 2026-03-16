namespace Application.Features.TaxConfigurations;

public record TaxConfigurationDto(
    Guid Id,
    string Name,
    decimal Percentage,
    int TaxType,
    string TaxTypeName,
    Guid DebitAccountId,
    string DebitAccountName,
    Guid CreditAccountId,
    string CreditAccountName
);
