using Domain.Entities.Users;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Sessions;

public class Session : AggregateRoot
{
    public Session() { }

    public Session(
        SessionId id,
        UserId userId,
        string deviceId,
        string token,
        AuditField auditField)
    {
        Id = id;
        UserId = userId;
        DeviceId = deviceId;
        Token = token;
        AuditField = auditField;
    }

    public SessionId Id { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public string DeviceId { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public AuditField AuditField { get; private set; } = default!;

    public void UpdateRefreshToken(string token)
    {
        Token = token;
    }
}