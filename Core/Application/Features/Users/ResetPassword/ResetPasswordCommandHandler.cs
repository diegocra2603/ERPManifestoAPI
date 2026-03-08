using Application.Helpers;
using Domain.Contracts.Identity;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Email;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBCryptService _bcryptService;
    private readonly IEmailService _emailService;
    private readonly IUserCurrentSession _userCurrentSession;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IBCryptService bcryptService,
        IEmailService emailService,
        IUserCurrentSession userCurrentSession)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _bcryptService = bcryptService;
        _emailService = emailService;
        _userCurrentSession = userCurrentSession;
    }

    public async Task<ErrorOr<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId ?? _userCurrentSession.Id;

        if (userId == Guid.Empty)
        {
            return Error.Unauthorized(
                code: "Auth.NotAuthenticated",
                description: "No se pudo identificar al usuario. Proporcione un UserId o inicie sesión.");
        }

        var userIdTyped = new UserId(userId);

        if (await _userRepository.FirstOrDefaultAsync(u => u.Id == userIdTyped) is not User user)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "Usuario no encontrado.");
        }

        var temporalPassword = PasswordHelper.GenerateTemporalPassword();
        
        var passwordHash = _bcryptService.HashPassword(temporalPassword);

        user.SetTemporalPassword(passwordHash);

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var emailResult = await _emailService.SendTemporalPasswordAsync(
            user.Email.Value,
            new Dictionary<string, string>
            {
                ["UserName"] = user.Name,
                ["TemporalPassword"] = temporalPassword
            });

        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        return Unit.Value;
    }
}
