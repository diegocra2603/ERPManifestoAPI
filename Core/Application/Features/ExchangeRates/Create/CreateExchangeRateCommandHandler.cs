using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.ExchangeRates.Create;

public class CreateExchangeRateCommandHandler : IRequestHandler<CreateExchangeRateCommand, ErrorOr<ExchangeRateDto>>
{
    private readonly IAsyncRepository<ExchangeRate> _exchangeRateRepository;
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExchangeRateCommandHandler(
        IAsyncRepository<ExchangeRate> exchangeRateRepository,
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _exchangeRateRepository = exchangeRateRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<ExchangeRateDto>> Handle(CreateExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.FirstOrDefaultAsync(c => c.Id == new CurrencyId(request.CurrencyId) && c.AuditField.IsActive);
        if (currency is null)
            return Error.NotFound("Currency.NotFound", "Moneda no encontrada.");

        if (await _exchangeRateRepository.ExistsAsync(e =>
            e.CurrencyId == new CurrencyId(request.CurrencyId) &&
            e.Date == ToUtc(request.Date) &&
            e.AuditField.IsActive))
        {
            return Error.Validation("ExchangeRate.DuplicateEntry", "Ya existe una tasa de cambio para esta moneda en la fecha indicada.");
        }

        var exchangeRate = new ExchangeRate(
            new ExchangeRateId(Guid.NewGuid()),
            new CurrencyId(request.CurrencyId),
            ToUtc(request.Date),
            request.BuyRate,
            request.SellRate,
            request.Source,
            AuditField.Create()
        );

        _exchangeRateRepository.Add(exchangeRate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExchangeRateDto(
            exchangeRate.Id.Value,
            currency.Id.Value,
            currency.Code,
            currency.Name,
            exchangeRate.Date,
            exchangeRate.BuyRate,
            exchangeRate.SellRate,
            exchangeRate.Source
        );
    }
}
