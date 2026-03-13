using Domain.Entities.Briefs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Update;

public record UpdateBriefCommand(
    Guid Id,
    string ClientName,
    string ContactName,
    DateTime Date,
    string? CompanyBackground,
    string? BrandingElementsToPreserve,
    string? CommunicationProblems,
    string? BrandPerception,
    DateTime? StartDate,
    DateTime? DeliveryDate,
    int? DurationMonths,
    decimal? Budget,
    List<UpdateBriefItemCommand> Items) : IRequest<ErrorOr<BriefDto>>;

public record UpdateBriefItemCommand(
    BriefSectionType SectionType,
    string ItemName,
    bool IsSelected,
    string? Comments);
