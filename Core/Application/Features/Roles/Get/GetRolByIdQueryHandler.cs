using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Roles;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Roles.Get;

public class GetRolByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ErrorOr<RoleDto>>
{
    private readonly IAsyncRepository<Role> _roleRepository;

    public GetRolByIdQueryHandler(IAsyncRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ErrorOr<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.Id);

        if (await _roleRepository.FirstOrDefaultAsync(role => role.Id == roleId) is not Role role)
        {
            return Error.NotFound(code: "Role.NotFound", description: "Role not found");
        }

        var response = new RoleDto(
            role.Id.Value,
            role.Name,
            role.Description
        );

        return response;
    }
}