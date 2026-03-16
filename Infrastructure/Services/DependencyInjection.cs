using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Contracts.Infrastructure.Services.BCrypt;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Email;
using Domain.Contracts.Infrastructure.Services.FiscalDataValidator;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Contracts.Infrastructure.Services.Files;
using Domain.Contracts.Infrastructure.Services.ImageCompressor;
using Domain.Contracts.Infrastructure.Services.InvoiceAccounting;
using Domain.Contracts.Infrastructure.Services.InvoicePdf;
using Services.BCrypt;
using Services.Token;
using Services.Email;
using Services.FiscalDataValidator;
using Services.FiscalDocument;
using Services.Files;
using Services.ImageCompressor;
using Services.InvoiceAccounting;
using Services.InvoicePdf;

namespace Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BCryptConfiguration>(configuration.GetSection(BCryptConfiguration.SectionName));
        services.Configure<TokenConfiguration>(configuration.GetSection(TokenConfiguration.SectionName));
        services.Configure<EmailConfiguration>(configuration.GetSection(EmailConfiguration.SectionName));
        services.Configure<FilesConfiguration>(configuration.GetSection(FilesConfiguration.SectionName));
        services.Configure<FiscalDataValidatorConfiguration>(configuration.GetSection(FiscalDataValidatorConfiguration.SectionName));
        services.Configure<FiscalDocumentConfiguration>(configuration.GetSection(FiscalDocumentConfiguration.SectionName));

        // Registrar TokenConfiguration para inyección directa (p. ej. en handlers de Auth)
        var tokenConfig = configuration.GetSection(TokenConfiguration.SectionName).Get<TokenConfiguration>();
        if (tokenConfig == null)
            throw new InvalidOperationException($"La sección de configuración '{TokenConfiguration.SectionName}' es requerida.");
        services.AddSingleton(tokenConfig);

        services.AddScoped<IBCryptService, BCryptService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IImageCompressorService, ImageCompressorService>();
        services.AddScoped<IFileService, FilesService>();

        services.AddHttpClient();
        services.AddScoped<IFiscalDataValidatorService, InnovaFiscalDataValidatorService>();
        services.AddScoped<IFiscalDocumentService, InnovaFiscalDocumentService>();
        services.AddScoped<IInvoicePdfService, InvoicePdfService>();
        services.AddScoped<IInvoiceAccountingService, InvoiceAccountingService>();

        return services;
    }
}
