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
    public string NombreComercial { get; set; } = default!;
    public string DireccionEmisor { get; set; } = default!;
    public string NitCertificador { get; set; } = default!;
    public string NombreCertificador { get; set; } = default!;
    public bool SujetoIsrTrimestral { get; set; }
}
