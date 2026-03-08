using System.Net;
using System.Net.Mail;
using Domain.Contracts.Infrastructure.Services.Email;
using Domain.Primitives.Mediator;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _configuration;
    private const string LogoUrl = "https://res.cloudinary.com/dlg9nca2v/image/upload/v1770315265/logo_sin_fondo_lwtj5s.png";

    public EmailService(IOptions<EmailConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task<ErrorOr<Unit>> SendWelcomeEmailAsync(string to, Dictionary<string, string> parameters)
    {
        var userName = parameters.GetValueOrDefault("UserName", "Usuario");

        var content = $@"
            <h1 style=""color:#1a1a1a;font-size:24px;font-weight:600;text-align:center;margin:0 0 24px 0;"">
                ¡Bienvenido a nuestra plataforma!
            </h1>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Hola <strong>{userName}</strong>,
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Tu cuenta ha sido creada exitosamente. Estamos encantados de tenerte con nosotros.
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 24px 0;"">
                Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.
            </p>";

        var html = BuildEmailTemplate("Bienvenido", content);
        return await SendEmailAsync(to, "Bienvenido a nuestra plataforma", html);
    }

    public async Task<ErrorOr<Unit>> SendTemporalPasswordAsync(string to, Dictionary<string, string> parameters)
    {
        var userName = parameters.GetValueOrDefault("UserName", "Usuario");
        var temporalPassword = parameters.GetValueOrDefault("TemporalPassword", "");

        var content = $@"
            <h1 style=""color:#1a1a1a;font-size:24px;font-weight:600;text-align:center;margin:0 0 24px 0;"">
                Contraseña Temporal
            </h1>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Hola <strong>{userName}</strong>,
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Se ha generado una contraseña temporal para tu cuenta:
            </p>
            <div style=""background-color:#f5f5f5;border:1px solid #e0e0e0;border-radius:8px;padding:20px;text-align:center;margin:24px 0;"">
                <p style=""font-size:24px;font-weight:700;color:#1a1a1a;margin:0;font-family:monospace;letter-spacing:2px;"">
                    {temporalPassword}
                </p>
            </div>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Por seguridad, te recomendamos cambiar esta contraseña inmediatamente después de iniciar sesión.
            </p>
            <p style=""font-size:14px;line-height:20px;color:#666666;margin:24px 0 0 0;"">
                Si no solicitaste esta contraseña temporal, por favor contacta a soporte inmediatamente.
            </p>";

        var html = BuildEmailTemplate("Contraseña Temporal", content);
        return await SendEmailAsync(to, "Tu contraseña temporal", html);
    }

    public async Task<ErrorOr<Unit>> SendResetPasswordEmailAsync(string to, Dictionary<string, string> parameters)
    {
        var userName = parameters.GetValueOrDefault("UserName", "Usuario");
        var resetLink = parameters.GetValueOrDefault("ResetLink", "#");

        var content = $@"
            <h1 style=""color:#1a1a1a;font-size:24px;font-weight:600;text-align:center;margin:0 0 24px 0;"">
                Restablecer Contraseña
            </h1>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Hola <strong>{userName}</strong>,
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 24px 0;"">
                Haz clic en el siguiente botón para crear una nueva contraseña:
            </p>
            <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""margin:24px 0;"">
                <tbody>
                    <tr>
                        <td align=""center"">
                            <a href=""{resetLink}"" 
                               style=""background-color:#1a1a1a;border-radius:8px;color:#ffffff;font-size:16px;font-weight:600;line-height:100%;text-decoration:none;text-align:center;padding:14px 32px;display:inline-block;"" 
                               target=""_blank"">
                                Restablecer Contraseña
                            </a>
                        </td>
                    </tr>
                </tbody>
            </table>
            <p style=""font-size:14px;line-height:20px;color:#666666;margin:24px 0 0 0;"">
                Este enlace expirará en 24 horas. Si no solicitaste restablecer tu contraseña, puedes ignorar este correo.
            </p>";

        var html = BuildEmailTemplate("Restablecer Contraseña", content);
        return await SendEmailAsync(to, "Restablecer tu contraseña", html);
    }

    public async Task<ErrorOr<Unit>> SendNotificationEmailAsync(string to, Dictionary<string, string> parameters)
    {
        var title = parameters.GetValueOrDefault("Title", "Notificación");
        var message = parameters.GetValueOrDefault("Message", "");
        var userName = parameters.GetValueOrDefault("UserName", "Usuario");

        var content = $@"
            <h1 style=""color:#1a1a1a;font-size:24px;font-weight:600;text-align:center;margin:0 0 24px 0;"">
                {title}
            </h1>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                Hola <strong>{userName}</strong>,
            </p>
            <p style=""font-size:16px;line-height:24px;color:#333333;margin:0 0 16px 0;"">
                {message}
            </p>";

        var html = BuildEmailTemplate(title, content);
        return await SendEmailAsync(to, title, html);
    }

    /// <summary>
    /// Construye el template HTML base del email con el contenido proporcionado.
    /// </summary>
    private static string BuildEmailTemplate(string previewText, string bodyContent)
    {
        return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>{previewText}</title>
    <link href=""https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600;700&display=swap"" rel=""stylesheet"">
    <!--[if mso]>
    <style type=""text/css"">
        body, table, td {{font-family: Arial, Helvetica, sans-serif !important;}}
    </style>
    <![endif]-->
</head>
<body style=""background-color:#f9f9f9;margin:0;padding:0;font-family:'Open Sans',-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue',sans-serif;"">
    <!-- Preview text -->
    <div style=""display:none;overflow:hidden;line-height:1px;opacity:0;max-height:0;max-width:0;"">
        {previewText}
    </div>

    <!-- Email container -->
    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color:#f9f9f9;padding:40px 20px;"">
        <tbody>
            <tr>
                <td align=""center"">
                    <!-- Main content card -->
                    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" 
                           style=""max-width:600px;background-color:#ffffff;border:1px solid #e5e5e5;border-radius:12px;overflow:hidden;"">
                        <tbody>
                            <tr>
                                <td>
                                    <!-- Header with logo (logo dentro de círculo blanco) -->
                                    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" 
                                           style=""background-color:#1a1a1a;padding:32px 40px;"">
                                        <tbody>
                                            <tr>
                                                <td align=""center"">
                                                    <div style=""display:inline-block;background-color:#ffffff;border-radius:50%;padding:16px;"">
                                                        <img alt=""Logo"" 
                                                             src=""{LogoUrl}"" 
                                                             width=""140"" 
                                                             height=""auto"" 
                                                             style=""display:block;outline:none;border:none;text-decoration:none;max-width:140px;"">
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <!-- Body content -->
                                    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" 
                                           style=""padding:40px;"">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    {bodyContent}
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <!-- Divider -->
                                    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" 
                                           style=""padding:0 40px;"">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <hr style=""border:none;border-top:1px solid #e5e5e5;margin:0;"">
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <!-- Copyright -->
                    <table align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" 
                           style=""max-width:600px;padding:24px 0;"">
                        <tbody>
                            <tr>
                                <td align=""center"">
                                    <p style=""font-size:12px;line-height:16px;color:#999999;margin:0;"">
                                        © {DateTime.UtcNow.Year} Todos los derechos reservados.
                                    </p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</body>
</html>";
    }

    /// <summary>
    /// Envía un email usando la configuración SMTP.
    /// </summary>
    private async Task<ErrorOr<Unit>> SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            // Puerto 25: sin SSL. Puerto 465: SSL. Puerto 587: STARTTLS (EnableSsl = true).
            var useSsl = _configuration.SmtpPort is 465 or 587;

            using var smtpClient = new SmtpClient(_configuration.SmtpHost, _configuration.SmtpPort)
            {
                Credentials = new NetworkCredential(_configuration.SmtpUsername, _configuration.SmtpPassword),
                EnableSsl = useSsl,
                Timeout = 15000, // 15 segundos para evitar que se quede congelado
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration.SmtpFromEmail, _configuration.SmtpFromDisplayName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            // ConfigureAwait(false) evita deadlocks en contextos async de ASP.NET
            await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "Email.SendFailed",
                description: $"Error al enviar el correo: {ex.Message}");
        }
    }
}
