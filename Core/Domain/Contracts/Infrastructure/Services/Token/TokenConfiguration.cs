namespace Domain.Contracts.Infrastructure.Services.Token;

public class TokenConfiguration
{
    public const string SectionName = "Auth";
    public string SecretKey { get; set; } = default!;
    public int TokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
}