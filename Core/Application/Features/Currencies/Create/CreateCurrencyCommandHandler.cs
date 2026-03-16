using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Currencies.Create;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, ErrorOr<CurrencyDto>>
{
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCurrencyCommandHandler(
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CurrencyDto>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        if (await _currencyRepository.ExistsAsync(c => c.Code == request.Code && c.AuditField.IsActive))
        {
            return Error.Validation("Currency.CodeAlreadyExists", "Ya existe una moneda con ese código.");
        }

        if (request.IsFunctional &&
            await _currencyRepository.ExistsAsync(c => c.IsFunctional && c.AuditField.IsActive))
        {
            return Error.Validation("Currency.FunctionalAlreadyExists", "Ya existe una moneda funcional. Solo puede haber una.");
        }

        var currency = new Currency(
            new CurrencyId(Guid.NewGuid()),
            request.Code,
            request.Name,
            request.Symbol,
            request.IsFunctional,
            request.DecimalPlaces,
            AuditField.Create()
        );

        _currencyRepository.Add(currency);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CurrencyDto(
            currency.Id.Value,
            currency.Code,
            currency.Name,
            currency.Symbol,
            currency.IsFunctional,
            currency.DecimalPlaces
        );
    }
}
