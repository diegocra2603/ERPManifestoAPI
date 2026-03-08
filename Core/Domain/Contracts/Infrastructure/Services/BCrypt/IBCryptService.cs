namespace Domain.Contracts.Infrastructure.Services.BCrypt;

public interface IBCryptService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}