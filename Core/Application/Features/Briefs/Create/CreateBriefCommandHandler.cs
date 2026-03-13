using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Briefs;
using Domain.Entities.Products;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Briefs.Create;

public class CreateBriefCommandHandler : IRequestHandler<CreateBriefCommand, ErrorOr<BriefDto>>
{
    private readonly IAsyncRepository<Brief> _briefRepository;
    private readonly IAsyncRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBriefCommandHandler(
        IAsyncRepository<Brief> briefRepository,
        IAsyncRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _briefRepository = briefRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<BriefDto>> Handle(CreateBriefCommand request, CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);

        if (await _productRepository.FirstOrDefaultAsync(p => p.Id == productId && p.AuditField.IsActive) is not Product product)
        {
            return Error.NotFound("Product.NotFound", "Producto no encontrado.");
        }

        var brief = new Brief(
            new BriefId(Guid.NewGuid()),
            productId,
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
            request.Budget,
            AuditField.Create()
        );

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

        _briefRepository.Add(brief);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BriefDto(
            brief.Id.Value,
            brief.ProductId.Value,
            product.Name,
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
