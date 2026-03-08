using Domain.Contracts.Identity;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBCryptService _bcryptService;
    private readonly IUserCurrentSession _userCurrentSession;

    public ChangePasswordCommandHandler(
        IAsyncRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IBCryptService bcryptService,
        IUserCurrentSession userCurrentSession)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _bcryptService = bcryptService;
        _userCurrentSession = userCurrentSession;
    }

    public async Task<ErrorOr<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userCurrentSession.Id;

        if (userId == Guid.Empty)
            return Error.Unauthorized(code: "Auth.NotAuthenticated", description: "No se pudo identificar al usuario.");

        var userIdTyped = new UserId(userId);

        var users = await _userRepository.GetAsync(
            predicate: u => u.Id == userIdTyped,
            orderBy: null,
            includeString: null,
            disableTracking: false
        );
        var user = users.FirstOrDefault(u => u.AuditField.DeletedAt == null);

        if (user is null)
            return Error.NotFound(code: "Auth.UserNotFound", description: "Usuario no encontrado.");

        if (!_bcryptService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Error.Validation(code: "Auth.InvalidCurrentPassword", description: "La contraseña actual es incorrecta.");

        var newPasswordHash = _bcryptService.HashPassword(request.NewPassword);
        user.SetNewPassword(newPasswordHash);
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
