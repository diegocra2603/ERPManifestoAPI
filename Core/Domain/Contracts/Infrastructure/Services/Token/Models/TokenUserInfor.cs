namespace Domain.Contracts.Infrastructure.Services.Token.Models;

public record TokenUserInfo(
    Guid UserId,
    string Username,
    Guid RoleId,
    string RoleName,
    IReadOnlyList<string> Permissions
);