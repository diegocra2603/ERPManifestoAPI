using Domain.Contracts.Infrastructure.Services.Token.Models;

namespace Domain.Contracts.Infrastructure.Services.Token;

public interface ITokenService
{
    string GenerateAccessToken(TokenUserInfo userInfo);
    string GenerateRefreshToken();
    bool ValidateAccessToken(string token);
    Guid? GetUserIdFromToken(string token);
    DateTime GetTokenExpirationDate(string token);

}
