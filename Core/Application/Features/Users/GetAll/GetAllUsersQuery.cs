using Application.Features.Users.DTOs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.GetAll;

public record GetAllUsersQuery : IRequest<ErrorOr<IReadOnlyList<UserDto>>>;
