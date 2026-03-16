using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.GetAll;

public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, ErrorOr<IReadOnlyList<CurrencyDto>>>
{
    private readonly IAsyncRepository<Currency> _currencyRepository;

    public GetAllCurrenciesQueryHandler(IAsyncRepository<Currency> currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<CurrencyDto>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _currencyRepository.GetAsync(c => c.AuditField.IsActive);

        return currencies
            .Select(c => new CurrencyDto(c.Id.Value, c.Code, c.Name, c.Symbol, c.IsFunctional, c.DecimalPlaces))
            .ToList()
            .AsReadOnly();
    }
}
