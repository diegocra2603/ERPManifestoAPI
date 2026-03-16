using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.GetAll;

public class GetAllTaxConfigurationsQueryHandler : IRequestHandler<GetAllTaxConfigurationsQuery, ErrorOr<IReadOnlyList<TaxConfigurationDto>>>
{
    private readonly IAsyncRepository<TaxConfiguration> _taxConfigurationRepository;

    public GetAllTaxConfigurationsQueryHandler(IAsyncRepository<TaxConfiguration> taxConfigurationRepository)
    {
        _taxConfigurationRepository = taxConfigurationRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<TaxConfigurationDto>>> Handle(GetAllTaxConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var includes = new List<Expression<Func<TaxConfiguration, object>>>
        {
            t => t.DebitAccount,
            t => t.CreditAccount
        };

        var taxConfigurations = await _taxConfigurationRepository.GetAsync(
            predicate: t => t.AuditField.IsActive,
            includes: includes);

        return taxConfigurations
            .Select(t => new TaxConfigurationDto(
                t.Id.Value,
                t.Name,
                t.Percentage,
                (int)t.TaxType,
                GetTaxTypeName(t.TaxType),
                t.DebitAccountId.Value,
                t.DebitAccount.Name,
                t.CreditAccountId.Value,
                t.CreditAccount.Name
            ))
            .ToList()
            .AsReadOnly();
    }

    private static string GetTaxTypeName(TaxType taxType) => taxType switch
    {
        TaxType.SobreVenta => "Sobre Venta",
        TaxType.SobreCompra => "Sobre Compra",
        TaxType.Retencion => "Retención",
        _ => taxType.ToString()
    };
}
