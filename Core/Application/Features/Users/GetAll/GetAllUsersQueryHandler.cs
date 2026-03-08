using Application.Features.Users.DTOs;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Entities.Roles;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.GetAll;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllWithRoleAsync();

        return users
            .Select(user => new UserDto(
                user.Id.Value,
                user.Email.Value,
                user.Name,
                user.Code,
                user.PhoneNumber.Value,
                user.BiometricEnabled,
                user.IsEmailConfirmed,
                user.AuditField.IsActive,
                new RoleDto(user.Role.Id.Value, user.Role.Name, user.Role.Description)
            ))
            .ToList()
            .AsReadOnly();
    }
}
