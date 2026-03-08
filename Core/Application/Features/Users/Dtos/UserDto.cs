using Domain.Entities.Roles;

namespace Application.Features.Users.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    string Code,
    string PhoneNumber,
    bool BiometricEnabled,
    bool ConfirmEmail,
    bool IsActive,
    RoleDto Role
);