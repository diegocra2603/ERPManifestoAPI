using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Roles;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Roles.GetAll;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ErrorOr<IReadOnlyList<RoleDto>>>
{
    private readonly IAsyncRepository<Role> _roleRepository;

    public GetAllRolesQueryHandler(IAsyncRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(role => new RoleDto(role.Id.Value, role.Name, role.Description)).ToList().AsReadOnly();
    }
}