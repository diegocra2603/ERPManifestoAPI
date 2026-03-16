using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.ExchangeRates.Update;

public class UpdateExchangeRateCommandHandler : IRequestHandler<UpdateExchangeRateCommand, ErrorOr<ExchangeRateDto>>
{
    private readonly IAsyncRepository<ExchangeRate> _exchangeRateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExchangeRateCommandHandler(
        IAsyncRepository<ExchangeRate> exchangeRateRepository,
        IUnitOfWork unitOfWork)
    {
        _exchangeRateRepository = exchangeRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ExchangeRateDto>> Handle(UpdateExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var rates = await _exchangeRateRepository.GetAsync(
            predicate: e => e.Id == new ExchangeRateId(request.Id) && e.AuditField.IsActive,
            includeString: "Currency");

        var exchangeRate = rates.FirstOrDefault();
        if (exchangeRate is null)
            return Error.NotFound("ExchangeRate.NotFound", "Tasa de cambio no encontrada.");

        exchangeRate.Update(request.BuyRate, request.SellRate, request.Source);
        _exchangeRateRepository.Update(exchangeRate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExchangeRateDto(
            exchangeRate.Id.Value,
            exchangeRate.CurrencyId.Value,
            exchangeRate.Currency.Code,
            exchangeRate.Currency.Name,
            exchangeRate.Date,
            exchangeRate.BuyRate,
            exchangeRate.SellRate,
            exchangeRate.Source
        );
    }
}
