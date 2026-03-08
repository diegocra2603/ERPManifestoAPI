namespace Application.Features.Auth.Dtos;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    bool EmailConfirmed,
    string SessionId,
    UserAuthDto User
);