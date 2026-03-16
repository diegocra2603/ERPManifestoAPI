using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.GetAll;

public class GetAllAccountCatalogsQueryHandler : IRequestHandler<GetAllAccountCatalogsQuery, ErrorOr<IReadOnlyList<AccountCatalogDto>>>
{
    private readonly IAsyncRepository<AccountCatalog> _accountCatalogRepository;

    public GetAllAccountCatalogsQueryHandler(IAsyncRepository<AccountCatalog> accountCatalogRepository)
    {
        _accountCatalogRepository = accountCatalogRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<AccountCatalogDto>>> Handle(GetAllAccountCatalogsQuery request, CancellationToken cancellationToken)
    {
        var allAccounts = await _accountCatalogRepository.GetAsync(a => a.AuditField.IsActive);
        var accountList = allAccounts.ToList();

        var lookup = accountList.ToDictionary(a => a.Id.Value);

        var rootAccounts = accountList
            .Where(a => a.ParentId == null)
            .Select(a => BuildTree(a, accountList, lookup))
            .ToList()
            .AsReadOnly();

        return rootAccounts;
    }

    private static AccountCatalogDto BuildTree(AccountCatalog account, List<AccountCatalog> allAccounts, Dictionary<Guid, AccountCatalog> lookup)
    {
        var children = allAccounts
            .Where(a => a.ParentId != null && a.ParentId.Value == account.Id.Value)
            .Select(child => BuildTree(child, allAccounts, lookup))
            .ToList()
            .AsReadOnly();

        string? parentName = null;
        if (account.ParentId != null && lookup.TryGetValue(account.ParentId.Value, out var parent))
        {
            parentName = parent.Name;
        }

        return new AccountCatalogDto(
            account.Id.Value,
            account.AccountCode,
            account.Name,
            (int)account.AccountType,
            account.AccountType.ToString(),
            (int)account.Nature,
            account.Nature.ToString(),
            account.ParentId?.Value,
            parentName,
            account.Level,
            account.AcceptsMovements,
            children
        );
    }
}
