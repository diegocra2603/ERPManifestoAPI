using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(request.UserId));

        if (user is null)
        {
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");
        }

        if (user.AuditField.IsDeleted)
        {
            return Error.Validation("User.AlreadyDeleted", "El usuario ya fue eliminado.");
        }

        user.MarkAsDeleted();

        _userRepository.Update(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
