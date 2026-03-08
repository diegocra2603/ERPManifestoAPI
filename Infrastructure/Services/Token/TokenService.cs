using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Constants;
using Domain.Contracts.Infrastructure.Services.Token;
using Domain.Contracts.Infrastructure.Services.Token.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Services.Token;

public class TokenService : ITokenService
{
    private readonly TokenConfiguration _config;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;

    public TokenService(IOptions<TokenConfiguration> config)
    {
        _config = config.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _config.Issuer,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public string GenerateAccessToken(TokenUserInfo userInfo)
    {
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.Uid, userInfo.UserId.ToString()),
            new(CustomClaimTypes.Username, userInfo.Username),
            new(CustomClaimTypes.RoleId, userInfo.RoleId.ToString()),
            new(CustomClaimTypes.RoleName, userInfo.RoleName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Agregar cada permiso como un claim separado
        foreach (var permission in userInfo.Permissions)
        {
            claims.Add(new Claim(CustomClaimTypes.Permission, permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.TokenExpirationMinutes),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials = _signingCredentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateAccessToken(string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, _validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out _);
            var userIdClaim = principal.FindFirst(CustomClaimTypes.Uid);
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public DateTime GetTokenExpirationDate(string token)
    {
        var principal = _tokenHandler.ValidateToken(token, _validationParameters, out _);
        var expirationClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp);
        return DateTime.UnixEpoch.AddSeconds(long.Parse(expirationClaim!.Value));
    }
}