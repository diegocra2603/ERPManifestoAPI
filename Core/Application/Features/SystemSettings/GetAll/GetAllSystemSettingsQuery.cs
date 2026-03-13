using Domain.Entities.SystemSettings;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.SystemSettings.GetAll;

public record GetAllSystemSettingsQuery() : IRequest<ErrorOr<IReadOnlyList<SystemSettingDto>>>;
