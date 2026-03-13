using Domain.Entities.Briefs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Create;

public record CreateBriefCommand(
    Guid ProductId,
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
    List<CreateBriefItemCommand> Items) : IRequest<ErrorOr<BriefDto>>;

public record CreateBriefItemCommand(
    BriefSectionType SectionType,
    string ItemName,
    bool IsSelected,
    string? Comments);
