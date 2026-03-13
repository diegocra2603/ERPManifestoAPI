using Domain.Entities.Briefs;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Get;

public record GetBriefByIdQuery(Guid Id) : IRequest<ErrorOr<BriefDto>>;
