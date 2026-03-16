using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.ExchangeRates.GetAll;

public class GetAllExchangeRatesQueryHandler : IRequestHandler<GetAllExchangeRatesQuery, ErrorOr<IReadOnlyList<ExchangeRateDto>>>
{
    private readonly IAsyncRepository<ExchangeRate> _exchangeRateRepository;

    public GetAllExchangeRatesQueryHandler(IAsyncRepository<ExchangeRate> exchangeRateRepository)
    {
        _exchangeRateRepository = exchangeRateRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<ExchangeRateDto>>> Handle(GetAllExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var exchangeRates = await _exchangeRateRepository.GetAsync(
            predicate: e => e.AuditField.IsActive,
            includeString: "Currency");

        return exchangeRates
            .Select(e => new ExchangeRateDto(
                e.Id.Value,
                e.CurrencyId.Value,
                e.Currency.Code,
                e.Currency.Name,
                e.Date,
                e.BuyRate,
                e.SellRate,
                e.Source))
            .ToList()
            .AsReadOnly();
    }
}
