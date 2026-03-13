using Domain.Primitives;

namespace Domain.Entities.Briefs;

public sealed class BriefItem : Entity
{
    private BriefItem() { }

    public BriefItem(
        BriefItemId id,
        BriefId briefId,
        BriefSectionType sectionType,
        string itemName,
        bool isSelected,
        string? comments)
    {
        Id = id;
        BriefId = briefId;
        SectionType = sectionType;
        ItemName = itemName;
        IsSelected = isSelected;
        Comments = comments;
    }

    public BriefItemId Id { get; private set; } = default!;
    public BriefId BriefId { get; private set; } = default!;
    public BriefSectionType SectionType { get; private set; }
    public string ItemName { get; private set; } = default!;
    public bool IsSelected { get; private set; }
    public string? Comments { get; private set; }

    // Navigation
    public Brief Brief { get; private set; } = default!;

    public void Update(bool isSelected, string? comments)
    {
        IsSelected = isSelected;
        Comments = comments;
    }
}
