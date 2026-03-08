using Application.Features.Users.DTOs;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Users.Get;

public class GetUserByParamQueryHandler : IRequestHandler<GetUserByParamQuery, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByParamQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserDto>> Handle(GetUserByParamQuery request, CancellationToken cancellationToken)
    {
        User? user = null;

        if (Guid.TryParse(request.Param, out var guid))
        {
            user = await _userRepository.GetByIdAsync(new UserId(guid));
        }

        if (user is null && Email.Create(request.Param) is Email email)
        {
            user = await _userRepository.GetByEmailAsync(email);
        }

        user ??= await _userRepository.GetByCodeAsync(request.Param);

        if (user is null)
        {
            return Error.NotFound(code: "User.NotFound", description: "User not found");
        }

        return new UserDto(
            user.Id.Value,
            user.Email.Value,
            user.Name,
            user.Code,
            user.PhoneNumber.Value,
            user.BiometricEnabled,
            user.IsEmailConfirmed,
            user.AuditField.IsActive,
            new RoleDto(user.Role.Id.Value, user.Role.Name, user.Role.Description)
        );
    }
}
