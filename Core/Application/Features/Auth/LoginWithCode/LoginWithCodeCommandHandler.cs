using Application.Features.Auth.Dtos;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Persistence.Repositories; 
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Token.Models;
using Domain.Entities.Sessions;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Auth.LoginWithCode;

public class LoginWithCodeCommandHandler : IRequestHandler<LoginWithCodeCommand, ErrorOr<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAsyncRepository<Session> _sessionRepository;
    private readonly IBCryptService _bcryptService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginWithCodeCommandHandler(IUserRepository userRepository, IAsyncRepository<Session> sessionRepository, IBCryptService bcryptService, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _bcryptService = bcryptService;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginWithCodeCommand request, CancellationToken cancellationToken)
    {

        if (await _userRepository.GetByCodeAsync(request.Code) is not User user)
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