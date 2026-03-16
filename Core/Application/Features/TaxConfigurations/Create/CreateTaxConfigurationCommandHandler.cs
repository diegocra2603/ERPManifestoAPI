using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.TaxConfigurations.Create;

public class CreateTaxConfigurationCommandHandler : IRequestHandler<CreateTaxConfigurationCommand, ErrorOr<TaxConfigurationDto>>
{
    private readonly IAsyncRepository<TaxConfiguration> _taxConfigurationRepository;
    private readonly IAsyncRepository<AccountCatalog> _accountCatalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaxConfigurationCommandHandler(
        IAsyncRepository<TaxConfiguration> taxConfigurationRepository,
        IAsyncRepository<AccountCatalog> accountCatalogRepository,
        IUnitOfWork unitOfWork)
    {
        _taxConfigurationRepository = taxConfigurationRepository;
        _accountCatalogRepository = accountCatalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<TaxConfigurationDto>> Handle(CreateTaxConfigurationCommand request, CancellationToken cancellationToken)
    {
        if (await _taxConfigurationRepository.ExistsAsync(t => t.Name == request.Name && t.AuditField.IsActive))
            return Error.Validation("TaxConfiguration.NameAlreadyExists", "Ya existe una configuración de impuesto con ese nombre.");

        var debitAccount = await _accountCatalogRepository.FirstOrDefaultAsync(a => a.Id == new AccountCatalogId(request.DebitAccountId) && a.AuditField.IsActive);
        if (debitAccount is null)
            return Error.NotFound("TaxConfiguration.DebitAccountNotFound", "La cuenta de débito no fue encontrada.");

        var creditAccount = await _accountCatalogRepository.FirstOrDefaultAsync(a => a.Id == new AccountCatalogId(request.CreditAccountId) && a.AuditField.IsActive);
        if (creditAccount is null)
            return Error.NotFound("TaxConfiguration.CreditAccountNotFound", "La cuenta de crédito no fue encontrada.");

        var taxConfiguration = new TaxConfiguration(
            new TaxConfigurationId(Guid.NewGuid()),
            request.Name,
            request.Percentage,
            (TaxType)request.TaxType,
            new AccountCatalogId(request.DebitAccountId),
            new AccountCatalogId(request.CreditAccountId),
            AuditField.Create()
        );

        _taxConfigurationRepository.Add(taxConfiguration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaxConfigurationDto(
            taxConfiguration.Id.Value,
            taxConfiguration.Name,
            taxConfiguration.Percentage,
            (int)taxConfiguration.TaxType,
            GetTaxTypeName(taxConfiguration.TaxType),
            debitAccount.Id.Value,
            debitAccount.Name,
            creditAccount.Id.Value,
            creditAccount.Name
        );
    }

    private static string GetTaxTypeName(TaxType taxType) => taxType switch
    {
        TaxType.SobreVenta => "Sobre Venta",
        TaxType.SobreCompra => "Sobre Compra",
        TaxType.Retencion => "Retención",
        _ => taxType.ToString()
    };
}
