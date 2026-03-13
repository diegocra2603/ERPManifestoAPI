using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Briefs;
using Domain.Entities.Products;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.GetByProduct;

public class GetBriefsByProductQueryHandler : IRequestHandler<GetBriefsByProductQuery, ErrorOr<IReadOnlyList<BriefDto>>>
{
    private readonly IAsyncRepository<Brief> _briefRepository;

    public GetBriefsByProductQueryHandler(IAsyncRepository<Brief> briefRepository)
    {
        _briefRepository = briefRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<BriefDto>>> Handle(GetBriefsByProductQuery request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);

        var briefs = await _briefRepository.GetAsync(
            predicate: b => b.ProductId == productId && b.AuditField.IsActive,
            includes: new()
            {
                b => b.Items,
                b => b.Product
            });

        return briefs
            .Select(b => new BriefDto(
                b.Id.Value,
                b.ProductId.Value,
                b.Product.Name,
                b.ClientName,
                b.ContactName,
                b.Date,
                b.CompanyBackground,
                b.BrandingElementsToPreserve,
                b.CommunicationProblems,
                b.BrandPerception,
                b.StartDate,
                b.DeliveryDate,
                b.DurationMonths,
                b.Budget,
                b.Items.Select(i => new BriefItemDto(
                    i.Id.Value,
                    i.SectionType,
                    i.ItemName,
                    i.IsSelected,
                    i.Comments
                )).ToList()
            ))
            .ToList()
            .AsReadOnly();
    }
}
