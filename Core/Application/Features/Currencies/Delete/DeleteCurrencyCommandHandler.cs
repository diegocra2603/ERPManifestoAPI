using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.Delete;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCurrencyCommandHandler(
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.FirstOrDefaultAsync(c => c.Id == new CurrencyId(request.Id) && c.AuditField.IsActive);
        if (currency is null)
            return Error.NotFound("Currency.NotFound", "Moneda no encontrada.");

        currency.MarkAsDeleted();
        _currencyRepository.Update(currency);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
