namespace Application.Features.Auth.Dtos;

public record UserAuthDto(
    Guid Id,
    string Email,
    string Name,
    string Code,
    string PhoneNumber,
    bool BiometricEnabled,
    bool ConfirmEmail,
    bool IsActive,
    RoleInfoDto Role
);
