using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.Update;

public record UpdateTaxConfigurationCommand(
    Guid Id,
    string Name,
    decimal Percentage,
    int TaxType,
    Guid DebitAccountId,
    Guid CreditAccountId) : IRequest<ErrorOr<TaxConfigurationDto>>;
