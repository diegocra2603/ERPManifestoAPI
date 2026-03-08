using Application.Features.Auth.Dtos;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Token.Models;
using Domain.Entities.Sessions;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAsyncRepository<Session> _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBCryptService _bcryptService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IAsyncRepository<Session> sessionRepository,
        IUnitOfWork unitOfWork,
        IBCryptService bcryptService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _bcryptService = bcryptService;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (Email.Create(request.Email) is not Email email)
        {
            return Error.Validation(code: "Email.Invalid", description: "El formato del correo electrónico no es válido.");
        }

        if (await _userRepository.GetByEmailAsync(email) is not User user)
        {
            return Error.NotFound(code: "Auth.UserNotFound", description: "User not found.");
        }

        if (!_bcryptService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Error.Validation(code: "Auth.InvalidPassword", description: "La contraseña no es válida.");
        }
        
        var accessToken = _tokenService.GenerateAccessToken(new TokenUserInfo(
            user.Id.Value,
            user.Name,
            user.Role.Id.Value,
            user.Role.Name,
            user.Role.Permissions.Select(p => p.Code).ToList()));

        var refreshToken = _tokenService.GenerateRefreshToken();

        // Sesión sin dispositivo (login simple): deviceId vacío
        const string emptyDeviceId = "";

        var existingSession = await _sessionRepository.FirstOrDefaultAsync(
            s => s.UserId == user.Id && s.DeviceId == emptyDeviceId
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
                emptyDeviceId,
                refreshToken,
                AuditField.Create()
            );
            _sessionRepository.Add(newSession);
            sessionId = newSession.Id.Value.ToString();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Calcular fecha de expiración
        var expiresAt = _tokenService.GetTokenExpirationDate(accessToken);

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
