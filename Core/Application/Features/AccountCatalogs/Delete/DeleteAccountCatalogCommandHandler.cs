using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.Delete;

public class DeleteAccountCatalogCommandHandler : IRequestHandler<DeleteAccountCatalogCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<AccountCatalog> _accountCatalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountCatalogCommandHandler(
        IAsyncRepository<AccountCatalog> accountCatalogRepository,
        IUnitOfWork unitOfWork)
    {
        _accountCatalogRepository = accountCatalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteAccountCatalogCommand request, CancellationToken cancellationToken)
    {
        var accountCatalog = await _accountCatalogRepository.FirstOrDefaultAsync(
            a => a.Id == new AccountCatalogId(request.Id) && a.AuditField.IsActive);
        if (accountCatalog is null)
            return Error.NotFound("AccountCatalog.NotFound", "Cuenta contable no encontrada.");

        if (await _accountCatalogRepository.ExistsAsync(
            a => a.ParentId == new AccountCatalogId(request.Id) && a.AuditField.IsActive))
            return Error.Validation("AccountCatalog.HasChildren", "No se puede eliminar una cuenta que tiene cuentas hijas.");

        accountCatalog.MarkAsDeleted();
        _accountCatalogRepository.Update(accountCatalog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
