using Domain.Entities.JobPositions;
using Domain.Entities.Roles;
using Domain.Entities.Sessions;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Users;

public sealed class User : AggregateRoot
{
    private User() { }

    public User(
        UserId id,
        Email email,
        string name,
        string code,
        PhoneNumber phoneNumber,
        RoleId roleId,
        JobPositionId? jobPositionId,
        string passwordHash,
        bool biometricEnabled,
        string publicKey,
        bool isEmailConfirmed,
        AuditField auditField)
    {
        Id = id;
        Email = email;
        Name = name;
        Code = code;
        PhoneNumber = phoneNumber;
        RoleId = roleId;
        JobPositionId = jobPositionId;
        PasswordHash = passwordHash;
        BiometricEnabled = biometricEnabled;
        PublicKey = publicKey;
        IsEmailConfirmed = isEmailConfirmed;
        AuditField = auditField;
    }

    public UserId Id { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public PhoneNumber PhoneNumber { get; private set; } = default!;
    public RoleId RoleId { get; private set; } = default!;
    public JobPositionId? JobPositionId { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public bool BiometricEnabled { get; private set; }
    public string PublicKey { get; private set; } = default!;
    public bool IsEmailConfirmed { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Relationships
    public IReadOnlyList<Session> Sessions { get; private set; } = default!;
    public Role Role { get; private set; } = default!;
    public JobPosition? JobPosition { get; private set; }

    public void SetNewPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        IsEmailConfirmed = true;
    }

    /// <summary>
    /// Establece una contraseña temporal sin marcar el email como confirmado.
    /// </summary>
    public void SetTemporalPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Actualiza los datos de perfil del usuario.
    /// </summary>
    public void UpdateProfile(Email email, string name, string code, PhoneNumber phoneNumber, RoleId roleId, JobPositionId? jobPositionId)
    {
        Email = email;
        Name = name;
        Code = code;
        PhoneNumber = phoneNumber;
        RoleId = roleId;
        JobPositionId = jobPositionId;
        AuditField = AuditField.Update();
    }

    /// <summary>
    /// Marca el usuario como eliminado (soft delete). Solo cambia el estado.
    /// </summary>
    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }

    public static User Update(
        UserId id,
        Email email,
        string name,
        string code,
        PhoneNumber phoneNumber,
        RoleId roleId,
        JobPositionId? jobPositionId,
        string passwordHash,
        bool biometricEnabled,
        string publicKey,
        bool isEmailConfirmed,
        AuditField auditField
    ) => new User(id, email, name, code, phoneNumber, roleId, jobPositionId, passwordHash, biometricEnabled, publicKey, isEmailConfirmed, auditField);
}
