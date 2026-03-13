using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Briefs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Get;

public class GetBriefByIdQueryHandler : IRequestHandler<GetBriefByIdQuery, ErrorOr<BriefDto>>
{
    private readonly IAsyncRepository<Brief> _briefRepository;

    public GetBriefByIdQueryHandler(IAsyncRepository<Brief> briefRepository)
    {
        _briefRepository = briefRepository;
    }

    public async Task<ErrorOr<BriefDto>> Handle(GetBriefByIdQuery request, CancellationToken cancellationToken)
    {
        var briefId = new BriefId(request.Id);

        var briefs = await _briefRepository.GetAsync(
            predicate: b => b.Id == briefId && b.AuditField.IsActive,
            includes: new()
            {
                b => b.Items,
                b => b.Product
            });

        var brief = briefs.FirstOrDefault();

        if (brief is null)
        {
            return Error.NotFound("Brief.NotFound", "Brief no encontrado.");
        }

        return new BriefDto(
            brief.Id.Value,
            brief.ProductId.Value,
            brief.Product.Name,
            brief.ClientName,
            brief.ContactName,
            brief.Date,
            brief.CompanyBackground,
            brief.BrandingElementsToPreserve,
            brief.CommunicationProblems,
            brief.BrandPerception,
            brief.StartDate,
            brief.DeliveryDate,
            brief.DurationMonths,
            brief.Budget,
            brief.Items.Select(i => new BriefItemDto(
                i.Id.Value,
                i.SectionType,
                i.ItemName,
                i.IsSelected,
                i.Comments
            )).ToList()
        );
    }
}
