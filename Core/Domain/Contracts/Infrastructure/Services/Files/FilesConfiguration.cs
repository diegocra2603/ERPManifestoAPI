namespace Domain.Contracts.Infrastructure.Services.Files;

public class FilesConfiguration
{
    public const string SectionName = "Files";
    public string CloudName { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string ApiSecret { get; set; } = default!;
    public string Folder { get; set; } = default!;
}