using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.Create;

public record CreateTaxConfigurationCommand(
    string Name,
    decimal Percentage,
    int TaxType,
    Guid DebitAccountId,
    Guid CreditAccountId) : IRequest<ErrorOr<TaxConfigurationDto>>;
