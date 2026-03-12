namespace Domain.Contracts.Infrastructure.Services.FiscalDataValidator;

public class FiscalDataValidatorConfiguration
{
    public const string SectionName = "FiscalDataValidator";
    public string Url { get; set; } = default!;
    public string UserHeader { get; set; } = default!;
    public string PasswordHeader { get; set; } = default!;
    public string Nit { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}
