namespace Domain.Contracts.Infrastructure.Services.FiscalDocument;

public class FiscalDocumentConfiguration
{
    public const string SectionName = "FiscalDocument";
    public string Url { get; set; } = default!;
    public string ContingencyUrl { get; set; } = default!;
    public string UserHeader { get; set; } = default!;
    public string PasswordHeader { get; set; } = default!;
    public string Nit { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
    public int Establecimiento { get; set; } = 1;
    public string IdMaquina { get; set; } = "1";
}
