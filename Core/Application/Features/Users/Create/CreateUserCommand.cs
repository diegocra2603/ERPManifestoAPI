using Application.Features.Users.DTOs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.Create;

public record CreateUserCommand(
    string Email,
    string Name,
    string Code,
    string PhoneNumber,
    Guid RoleId) : IRequest<ErrorOr<UserDto>>;