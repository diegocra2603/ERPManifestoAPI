using Domain.Entities.SystemSettings;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.SystemSettings.Update;

public record UpdateSystemSettingCommand(
    Guid Id,
    string Value) : IRequest<ErrorOr<SystemSettingDto>>;
