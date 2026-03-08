using Domain.Primitives.Mediator;
using ErrorOr;

namespace Domain.Contracts.Infrastructure.Services.Email;

public interface IEmailService
{
    Task<ErrorOr<Unit>> SendTemporalPasswordAsync(string to, Dictionary<string, string> parameters);
    Task<ErrorOr<Unit>> SendWelcomeEmailAsync(string to, Dictionary<string, string> parameters);
    Task<ErrorOr<Unit>> SendResetPasswordEmailAsync(string to, Dictionary<string, string> parameters);
    Task<ErrorOr<Unit>> SendNotificationEmailAsync(string to, Dictionary<string, string> parameters);
}