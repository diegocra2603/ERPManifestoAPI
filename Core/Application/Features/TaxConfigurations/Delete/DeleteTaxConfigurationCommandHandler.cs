using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.TaxConfigurations.Delete;

public class DeleteTaxConfigurationCommandHandler : IRequestHandler<DeleteTaxConfigurationCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<TaxConfiguration> _taxConfigurationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaxConfigurationCommandHandler(
        IAsyncRepository<TaxConfiguration> taxConfigurationRepository,
        IUnitOfWork unitOfWork)
    {
        _taxConfigurationRepository = taxConfigurationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteTaxConfigurationCommand request, CancellationToken cancellationToken)
    {
        var taxConfiguration = await _taxConfigurationRepository.FirstOrDefaultAsync(t => t.Id == new TaxConfigurationId(request.Id) && t.AuditField.IsActive);
        if (taxConfiguration is null)
            return Error.NotFound("TaxConfiguration.NotFound", "Configuración de impuesto no encontrada.");

        taxConfiguration.MarkAsDeleted();
        _taxConfigurationRepository.Update(taxConfiguration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
