using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.Update;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, ErrorOr<CurrencyDto>>
{
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCurrencyCommandHandler(
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CurrencyDto>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.FirstOrDefaultAsync(c => c.Id == new CurrencyId(request.Id) && c.AuditField.IsActive);
        if (currency is null)
            return Error.NotFound("Currency.NotFound", "Moneda no encontrada.");

        if (await _currencyRepository.ExistsAsync(c => c.Code == request.Code && c.Id != new CurrencyId(request.Id) && c.AuditField.IsActive))
            return Error.Validation("Currency.CodeAlreadyExists", "Ya existe otra moneda con ese código.");

        if (request.IsFunctional &&
            await _currencyRepository.ExistsAsync(c => c.IsFunctional && c.Id != new CurrencyId(request.Id) && c.AuditField.IsActive))
            return Error.Validation("Currency.FunctionalAlreadyExists", "Ya existe otra moneda funcional.");

        currency.Update(request.Code, request.Name, request.Symbol, request.IsFunctional, request.DecimalPlaces);
        _currencyRepository.Update(currency);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CurrencyDto(currency.Id.Value, currency.Code, currency.Name, currency.Symbol, currency.IsFunctional, currency.DecimalPlaces);
    }
}
