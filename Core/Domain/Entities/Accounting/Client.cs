using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class Client : AggregateRoot
{
    private Client() { }

    public Client(
        ClientId id,
        string name,
        string? legalName,
        string? nit,
        string? address,
        string? phone,
        string? email,
        AuditField auditField)
    {
        Id = id;
        Name = name;
        LegalName = legalName;
        Nit = nit;
        Address = address;
        Phone = phone;
        Email = email;
        AuditField = auditField;
    }

    public ClientId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? LegalName { get; private set; }
    public string? Nit { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    public void Update(string name, string? legalName, string? nit, string? address, string? phone, string? email)
    {
        Name = name;
        LegalName = legalName;
        Nit = nit;
        Address = address;
        Phone = phone;
        Email = email;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
