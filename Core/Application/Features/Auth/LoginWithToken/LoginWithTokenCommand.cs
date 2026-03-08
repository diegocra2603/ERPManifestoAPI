using Application.Features.Auth.Dtos;
using ErrorOr;
using Domain.Primitives.Mediator;

namespace Application.Features.Auth.LoginWithToken;
public record LoginWithTokenCommand : IRequest<ErrorOr<AuthResponse>>;
