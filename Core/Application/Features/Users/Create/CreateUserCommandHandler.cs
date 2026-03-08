using Application.Features.Users.DTOs;
using Application.Helpers;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Email;
using Domain.Entities.JobPositions;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Users.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAsyncRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBCryptService _bcryptService;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IAsyncRepository<Role> roleRepository,
        IUnitOfWork unitOfWork,
        IBCryptService bcryptService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _bcryptService = bcryptService;
        _emailService = emailService;
    }

    public async Task<ErrorOr<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.RoleId);

        if (await _roleRepository.FirstOrDefaultAsync(x => x.Id == roleId) is not Role role)
        {
            return Error.Validation("Role.NotFound", "Role not found");
        }

        // vaidar si el email es valido
        if (Email.Create(request.Email) is not Email email)
        {
            return Error.Validation("User.EmailInvalid", "Email is invalid");
        }

        // Validar si el email ya existe
        if (await _userRepository.FirstOrDefaultAsync(x => x.Email == email) is not null)
        {
            return Error.Validation("User.EmailAlreadyExists", "Email already exists");
        }

        var temporalPassword = PasswordHelper.GenerateTemporalPassword();

        var passwordHash = _bcryptService.HashPassword(temporalPassword);

        if (new User(
            new UserId(Guid.NewGuid()),
            email,
            request.Name,
            request.Code,
            new PhoneNumber(request.PhoneNumber),
            roleId,
            new JobPositionId(request.JobPositionId),
            passwordHash,
            false,
            string.Empty,
            false, // IsEmailConfirmed: email sin confirmar
            AuditField.Create()
        ) is not User user)
        {
            return Error.Validation("User.Invalid", "User is invalid");
        }

        _userRepository.Add(user);

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

        return new UserDto(
            user.Id.Value,
            user.Email.Value,
            user.Name,
            user.Code,
            user.PhoneNumber.Value,
            user.BiometricEnabled,
            user.IsEmailConfirmed,
            true, // IsActive - usuario recién creado
            new RoleDto(role.Id.Value, role.Name, role.Description)
        );
    }
}