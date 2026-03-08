using Application.Features.Auth.Dtos;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<AuthResponse>>;
