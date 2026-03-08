using Domain.Contracts.Infrastructure.Services.BCrypt;
using Microsoft.Extensions.Options;

namespace Services.BCrypt;

public class BCryptService : IBCryptService
{
    private readonly BCryptConfiguration _configuration;

    public BCryptService(IOptions<BCryptConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public string HashPassword(string password)
    {
        return global::BCrypt.Net.BCrypt.EnhancedHashPassword(password, _configuration.WorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return global::BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}