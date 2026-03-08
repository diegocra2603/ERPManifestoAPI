using Application.Features.Users.DTOs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.Update;

public record UpdateUserCommand(
    Guid Id,
    string Email,
    string Name,
    string Code,
    string PhoneNumber,
    Guid RoleId,
    Guid? StoreId = null,
    string? NeoNetToken = null) : IRequest<ErrorOr<UserDto>>;
