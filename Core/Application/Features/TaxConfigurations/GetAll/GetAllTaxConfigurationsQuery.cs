using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.GetAll;

public record GetAllTaxConfigurationsQuery : IRequest<ErrorOr<IReadOnlyList<TaxConfigurationDto>>>;
