using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.ExchangeRates.Delete;

public class DeleteExchangeRateCommandHandler : IRequestHandler<DeleteExchangeRateCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<ExchangeRate> _exchangeRateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExchangeRateCommandHandler(
        IAsyncRepository<ExchangeRate> exchangeRateRepository,
        IUnitOfWork unitOfWork)
    {
        _exchangeRateRepository = exchangeRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var exchangeRate = await _exchangeRateRepository.FirstOrDefaultAsync(e => e.Id == new ExchangeRateId(request.Id) && e.AuditField.IsActive);
        if (exchangeRate is null)
            return Error.NotFound("ExchangeRate.NotFound", "Tasa de cambio no encontrada.");

        exchangeRate.MarkAsDeleted();
        _exchangeRateRepository.Update(exchangeRate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
