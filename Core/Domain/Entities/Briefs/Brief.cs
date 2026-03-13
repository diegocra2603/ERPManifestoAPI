using Domain.Entities.Products;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Briefs;

public sealed class Brief : AggregateRoot
{
    private readonly List<BriefItem> _items = new();

    private Brief() { }

    public Brief(
        BriefId id,
        ProductId productId,
        string clientName,
        string contactName,
        DateTime date,
        string? companyBackground,
        string? brandingElementsToPreserve,
        string? communicationProblems,
        string? brandPerception,
        DateTime? startDate,
        DateTime? deliveryDate,
        int? durationMonths,
        decimal? budget,
        AuditField auditField)
    {
        Id = id;
        ProductId = productId;
        ClientName = clientName;
        ContactName = contactName;
        Date = date;
        CompanyBackground = companyBackground;
        BrandingElementsToPreserve = brandingElementsToPreserve;
        CommunicationProblems = communicationProblems;
        BrandPerception = brandPerception;
        StartDate = startDate;
        DeliveryDate = deliveryDate;
        DurationMonths = durationMonths;
        Budget = budget;
        AuditField = auditField;
    }

    public BriefId Id { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;

    // Datos del cliente
    public string ClientName { get; private set; } = default!;
    public string ContactName { get; private set; } = default!;
    public DateTime Date { get; private set; }

    // Información de la empresa
    public string? CompanyBackground { get; private set; }
    public string? BrandingElementsToPreserve { get; private set; }
    public string? CommunicationProblems { get; private set; }
    public string? BrandPerception { get; private set; }

    // Tiempos
    public DateTime? StartDate { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public int? DurationMonths { get; private set; }

    // Presupuesto
    public decimal? Budget { get; private set; }

    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public Product Product { get; private set; } = default!;
    public IReadOnlyCollection<BriefItem> Items => _items.AsReadOnly();

    public void Update(
        string clientName,
        string contactName,
        DateTime date,
        string? companyBackground,
        string? brandingElementsToPreserve,
        string? communicationProblems,
        string? brandPerception,
        DateTime? startDate,
        DateTime? deliveryDate,
        int? durationMonths,
        decimal? budget)
    {
        ClientName = clientName;
        ContactName = contactName;
        Date = date;
        CompanyBackground = companyBackground;
        BrandingElementsToPreserve = brandingElementsToPreserve;
        CommunicationProblems = communicationProblems;
        BrandPerception = brandPerception;
        StartDate = startDate;
        DeliveryDate = deliveryDate;
        DurationMonths = durationMonths;
        Budget = budget;
        AuditField = AuditField.Update();
    }

    public void AddItem(BriefItem item)
    {
        _items.Add(item);
    }

    public void ClearItems()
    {
        _items.Clear();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
