using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.SystemSettings;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.SystemSettings.Update;

public class UpdateSystemSettingCommandHandler
    : IRequestHandler<UpdateSystemSettingCommand, ErrorOr<SystemSettingDto>>
{
    private readonly IAsyncRepository<SystemSetting> _settingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSystemSettingCommandHandler(
        IAsyncRepository<SystemSetting> settingRepository,
        IUnitOfWork unitOfWork)
    {
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<SystemSettingDto>> Handle(
        UpdateSystemSettingCommand request,
        CancellationToken cancellationToken)
    {
        var settingId = new SystemSettingId(request.Id);

        var setting = await _settingRepository.FirstOrDefaultAsync(s => s.Id == settingId);

        if (setting is null)
        {
            return Error.NotFound(
                code: "SystemSetting.NotFound",
                description: "No se encontró la configuración del sistema.");
        }

        if (setting.AuditField.IsDeleted)
        {
            return Error.NotFound(
                code: "SystemSetting.Deleted",
                description: "La configuración del sistema ha sido eliminada.");
        }

        setting.UpdateValue(request.Value);

        _settingRepository.Update(setting);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SystemSettingDto(
            setting.Id.Value,
            setting.Key,
            setting.Value,
            setting.Description);
    }
}
