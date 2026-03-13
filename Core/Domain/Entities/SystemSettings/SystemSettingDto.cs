namespace Domain.Entities.SystemSettings;

public record SystemSettingDto(
    Guid Id,
    string Key,
    string Value,
    string Description);
