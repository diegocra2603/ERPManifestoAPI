using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.Update;

public class UpdateAccountCatalogCommandHandler : IRequestHandler<UpdateAccountCatalogCommand, ErrorOr<AccountCatalogDto>>
{
    private readonly IAsyncRepository<AccountCatalog> _accountCatalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountCatalogCommandHandler(
        IAsyncRepository<AccountCatalog> accountCatalogRepository,
        IUnitOfWork unitOfWork)
    {
        _accountCatalogRepository = accountCatalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AccountCatalogDto>> Handle(UpdateAccountCatalogCommand request, CancellationToken cancellationToken)
    {
        var accountCatalog = await _accountCatalogRepository.FirstOrDefaultAsync(
            a => a.Id == new AccountCatalogId(request.Id) && a.AuditField.IsActive);
        if (accountCatalog is null)
            return Error.NotFound("AccountCatalog.NotFound", "Cuenta contable no encontrada.");

        if (await _accountCatalogRepository.ExistsAsync(
            a => a.AccountCode == request.AccountCode && a.Id != new AccountCatalogId(request.Id) && a.AuditField.IsActive))
            return Error.Validation("AccountCatalog.CodeAlreadyExists", "Ya existe otra cuenta con ese código.");

        AccountCatalog? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _accountCatalogRepository.FirstOrDefaultAsync(
                a => a.Id == new AccountCatalogId(request.ParentId.Value) && a.AuditField.IsActive);
            if (parent is null)
                return Error.NotFound("AccountCatalog.ParentNotFound", "La cuenta padre no fue encontrada.");
        }

        accountCatalog.Update(
            request.AccountCode,
            request.Name,
            (AccountType)request.AccountType,
            (AccountNature)request.Nature,
            request.ParentId.HasValue ? new AccountCatalogId(request.ParentId.Value) : null,
            request.Level,
            request.AcceptsMovements
        );

        _accountCatalogRepository.Update(accountCatalog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountCatalogDto(
            accountCatalog.Id.Value,
            accountCatalog.AccountCode,
            accountCatalog.Name,
            (int)accountCatalog.AccountType,
            accountCatalog.AccountType.ToString(),
            (int)accountCatalog.Nature,
            accountCatalog.Nature.ToString(),
            request.ParentId,
            parent?.Name,
            accountCatalog.Level,
            accountCatalog.AcceptsMovements,
            new List<AccountCatalogDto>().AsReadOnly()
        );
    }
}
