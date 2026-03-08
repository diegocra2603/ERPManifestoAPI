using System.Security.Claims;

namespace Domain.Contracts.Identity;

public class UserCurrentSession : IUserCurrentSession
{
    public string Email { get; set; } = default!;
    public Guid Id { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
    public List<Claim> UserClaims { get; set; } = new List<Claim>();
    public string Username { get; set; } = default!;
}