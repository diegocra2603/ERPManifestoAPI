
using Application.Features.Auth.Dtos;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.LoginWithCode;

public record LoginWithCodeCommand(
    string Code,
    string Password
) : IRequest<ErrorOr<AuthResponse>>;