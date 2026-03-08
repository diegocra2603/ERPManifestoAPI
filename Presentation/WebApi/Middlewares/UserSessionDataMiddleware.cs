using Application.Common.Constants;
using Domain.Contracts.Identity;
using System.Security.Claims;

namespace WebApi.Middlewares;

public class UserSessionDataMiddleware
{
    private readonly RequestDelegate _next;

    public UserSessionDataMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserCurrentSession userSessionData)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var uidValue = context.User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (!string.IsNullOrEmpty(uidValue) && Guid.TryParse(uidValue, out var userId))
            {
                userSessionData.Id = userId;
            }
            userSessionData.Username = context.User.FindFirst(CustomClaimTypes.Username)?.Value ?? string.Empty;
            userSessionData.Email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            userSessionData.Roles = context.User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .Concat(context.User.FindAll(CustomClaimTypes.RoleName).Select(c => c.Value))
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct()
                .ToList();
            userSessionData.UserClaims = context.User.Claims.ToList();
        }

        await _next(context);
    }
}