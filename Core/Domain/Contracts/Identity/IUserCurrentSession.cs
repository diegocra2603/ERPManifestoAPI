using System.Security.Claims;

namespace Domain.Contracts.Identity;

public interface IUserCurrentSession
{
    Guid Id { get; set; }
    string Username { get; set; }
    string Email { get; set; }
    List<string> Roles { get; set; }
    List<Claim> UserClaims { get; set; }
}