namespace Domain.Contracts.Infrastructure.Services.Email;

public class EmailConfiguration
{
    public const string SectionName = "Email";
    public string SmtpFromEmail { get; set; } = default!;
    public string SmtpHost { get; set; } = default!;
    public int SmtpPort { get; set; } = default!;
    public string SmtpUsername { get; set; } = default!;
    public string SmtpPassword { get; set; } = default!;
    public string SmtpFromDisplayName { get; set; } = default!;
}