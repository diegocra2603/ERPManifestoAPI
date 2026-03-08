using Application.Features.Auth.Dtos;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Token.Models;
using Domain.Entities.Sessions;
using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.LoginWithDevice;

public class LoginWithDeviceCommandHandler : IRequestHandler<LoginWithDeviceCommand, ErrorOr<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAsyncRepository<Session> _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBCryptService _bcryptService;
    private readonly ITokenService _tokenService;
    private readonly TokenConfiguration _tokenConfiguration;

    public LoginWithDeviceCommandHandler(
        IUserRepository userRepository,
        IAsyncRepository<Session> sessionRepository,
        IUnitOfWork unitOfWork,
        IBCryptService bcryptService,
        ITokenService tokenService,
        TokenConfiguration tokenConfiguration)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _bcryptService = bcryptService;
        _tokenService = tokenService;
        _tokenConfiguration = tokenConfiguration;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginWithDeviceCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        if (email is null)
            return Error.Validation(code: "Email.Invalid", description: "El formato del correo electrónico no es válido.");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Credenciales inválidas.");

        if (!_bcryptService.VerifyPassword(request.Password, user.PasswordHash))
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Credenciales inválidas.");

        var permissions = user.Role.Permissions
            .Select(p => p.Code)
            .ToList();

        var tokenUserInfo = new TokenUserInfo(
            user.Id.Value,
            user.Name,
            user.Role.Id.Value,
            user.Role.Name,
            permissions
        );

        var accessToken = _tokenService.GenerateAccessToken(tokenUserInfo);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Sesión asociada al dispositivo
        var existingSession = await _sessionRepository.FirstOrDefaultAsync(
            s => s.UserId == user.Id && s.DeviceId == request.DeviceId
        );

        string sessionId;

        if (existingSession is not null)
        {
            existingSession.UpdateRefreshToken(refreshToken);
            _sessionRepository.Update(existingSession);
            sessionId = existingSession.Id.Value.ToString();
        }
        else
        {
            var newSession = new Session(
                new SessionId(Guid.NewGuid()),
                user.Id,
                request.DeviceId,
                refreshToken,
                AuditField.Create()
            );
            _sessionRepository.Add(newSession);
            sessionId = newSession.Id.Value.ToString();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return new AuthResponse(accessToken, expiresAt, user.IsEmailConfirmed, sessionId, userAuthDto);
    }
}
