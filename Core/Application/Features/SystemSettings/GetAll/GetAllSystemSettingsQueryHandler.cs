using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.SystemSettings;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.SystemSettings.GetAll;

public class GetAllSystemSettingsQueryHandler
    : IRequestHandler<GetAllSystemSettingsQuery, ErrorOr<IReadOnlyList<SystemSettingDto>>>
{
    private readonly IAsyncRepository<SystemSetting> _settingRepository;

    public GetAllSystemSettingsQueryHandler(IAsyncRepository<SystemSetting> settingRepository)
    {
        _settingRepository = settingRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<SystemSettingDto>>> Handle(
        GetAllSystemSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _settingRepository.GetAsync(s => s.AuditField.IsActive);

        var settingDtos = settings
            .Select(s => new SystemSettingDto(
                s.Id.Value,
                s.Key,
                s.Value,
                s.Description))
            .ToList()
            .AsReadOnly();

        return settingDtos;
    }
}
