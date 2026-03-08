using Application.Features.Users.DTOs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.CreateWithPassword;

public record CreateUserWithPasswordCommand(
    string Email,
    string Name,
    string Code,
    string PhoneNumber,
    Guid RoleId,
    string Password) : IRequest<ErrorOr<UserDto>>;
