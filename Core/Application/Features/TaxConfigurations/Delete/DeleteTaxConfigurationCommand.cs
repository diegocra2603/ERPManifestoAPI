using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.Delete;

public record DeleteTaxConfigurationCommand(Guid Id) : IRequest<ErrorOr<bool>>;
