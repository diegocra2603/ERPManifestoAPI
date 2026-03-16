using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class Supplier : AggregateRoot
{
    private Supplier() { }

    public Supplier(
        SupplierId id,
        string nit,
        string name,
        string? address,
        string? phone,
        string? email,
        AuditField auditField)
    {
        Id = id;
        Nit = nit;
        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        AuditField = auditField;
    }

    public SupplierId Id { get; private set; } = default!;
    public string Nit { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    public void Update(string nit, string name, string? address, string? phone, string? email)
    {
        Nit = nit;
        Name = name;
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
