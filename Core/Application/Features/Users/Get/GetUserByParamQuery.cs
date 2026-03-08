using Application.Features.Users.DTOs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.Get;

public record GetUserByParamQuery(string Param) : IRequest<ErrorOr<UserDto>>;
