using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Briefs;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Update;

public class UpdateBriefCommandHandler : IRequestHandler<UpdateBriefCommand, ErrorOr<BriefDto>>
{
    private readonly IAsyncRepository<Brief> _briefRepository;
    private readonly IAsyncRepository<BriefItem> _briefItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBriefCommandHandler(
        IAsyncRepository<Brief> briefRepository,
        IAsyncRepository<BriefItem> briefItemRepository,
        IUnitOfWork unitOfWork)
    {
        _briefRepository = briefRepository;
        _briefItemRepository = briefItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<BriefDto>> Handle(UpdateBriefCommand request, CancellationToken cancellationToken)
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

        brief.Update(
            request.ClientName,
            request.ContactName,
            request.Date,
            request.CompanyBackground,
            request.BrandingElementsToPreserve,
            request.CommunicationProblems,
            request.BrandPerception,
            request.StartDate,
            request.DeliveryDate,
            request.DurationMonths,
            request.Budget
        );

        // Eliminar items existentes y agregar los nuevos
        foreach (var existingItem in brief.Items.ToList())
        {
            _briefItemRepository.Delete(existingItem);
        }
        brief.ClearItems();

        foreach (var item in request.Items)
        {
            brief.AddItem(new BriefItem(
                new BriefItemId(Guid.NewGuid()),
                brief.Id,
                item.SectionType,
                item.ItemName,
                item.IsSelected,
                item.Comments
            ));
        }

        _briefRepository.Update(brief);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
