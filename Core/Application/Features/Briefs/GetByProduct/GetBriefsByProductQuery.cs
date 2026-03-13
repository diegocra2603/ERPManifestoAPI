using Domain.Entities.Briefs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.GetByProduct;

public record GetBriefsByProductQuery(Guid ProductId) : IRequest<ErrorOr<IReadOnlyList<BriefDto>>>;
