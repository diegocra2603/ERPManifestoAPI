using Application.Features.Auth.Dtos;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.LoginWithDevice;

public record LoginWithDeviceCommand(
    string Email,
    string Password,
    string DeviceId
) : IRequest<ErrorOr<AuthResponse>>;
