using Application.Features.Auth.Dtos;
using Domain.Contracts.Identity;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Token.Models;
using Domain.Entities.Users;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.LoginWithToken;

public class LoginWithTokenCommandHandler : IRequestHandler<LoginWithTokenCommand, ErrorOr<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly TokenConfiguration _tokenConfiguration;
    private readonly IUserCurrentSession _userCurrentSession;

    public LoginWithTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        TokenConfiguration tokenConfiguration,
        IUserCurrentSession userCurrentSession)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _tokenConfiguration = tokenConfiguration;
        _userCurrentSession = userCurrentSession;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginWithTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _userCurrentSession.Id;
        if (userId == Guid.Empty)
            return Error.Unauthorized(code: "Auth.NotAuthenticated", description: "No se pudo identificar al usuario.");

        var userIdTyped = new UserId(userId);

        var user = await _userRepository.GetByIdAsync(userIdTyped);

        if (user is null || !user.AuditField.IsActive || user.AuditField.DeletedAt != null)
            return Error.Unauthorized(code: "Auth.UserInactive", description: "Usuario no encontrado o inactivo.");

        var tokenUserInfo = new TokenUserInfo(
            user.Id.Value,
            user.Name,
            user.Role.Id.Value,
            user.Role.Name,
            user.Role.Permissions.Select(p => p.Code).ToList()
        );

        var accessToken = _tokenService.GenerateAccessToken(tokenUserInfo);
        var expiresAt = DateTime.UtcNow.AddMinutes(_tokenConfiguration.TokenExpirationMinutes);

        // Crear RoleInfoDto (sin permisos)
        var roleInfoDto = new RoleInfoDto(
            user.Role.Id.Value,
            user.Role.Name,
            user.Role.Description
        );

        // Crear UserAuthDto
        var userAuthDto = new UserAuthDto(
            user.Id.Value,
            user.Email.Value,
            user.Name,
            user.Code,
            user.PhoneNumber.Value,
            user.BiometricEnabled,
            user.IsEmailConfirmed,
            user.AuditField.IsActive,
            roleInfoDto
        );

        return new AuthResponse(accessToken, expiresAt, user.IsEmailConfirmed, string.Empty, userAuthDto);
    }
}
