using Application.Features.Users.DTOs;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Users.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAsyncRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IAsyncRepository<Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(request.Id));

        if (user is null)
        {
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");
        }

        if (user.AuditField.IsDeleted)
        {
            return Error.NotFound("User.NotFound", "El usuario ya fue eliminado.");
        }

        var roleId = new RoleId(request.RoleId);
        if (await _roleRepository.FirstOrDefaultAsync(r => r.Id == roleId) is not Role role)
        {
            return Error.Validation("Role.NotFound", "Rol no encontrado.");
        }

        if (Email.Create(request.Email) is not Email email)
        {
            return Error.Validation("User.EmailInvalid", "El email no es válido.");
        }

        var existingWithEmail = await _userRepository.GetByEmailAsync(email);
        if (existingWithEmail is not null && existingWithEmail.Id != user.Id)
        {
            return Error.Validation("User.EmailAlreadyExists", "El email ya está en uso por otro usuario.");
        }

        user.UpdateProfile(
            email,
            request.Name,
            request.Code,
            new PhoneNumber(request.PhoneNumber),
            roleId
            );

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id.Value,
            user.Email.Value,
            user.Name,
            user.Code,
            user.PhoneNumber.Value,
            user.BiometricEnabled,
            user.IsEmailConfirmed,
            user.AuditField.IsActive,
            new RoleDto(role.Id.Value, role.Name, role.Description)
        );
    }
}
