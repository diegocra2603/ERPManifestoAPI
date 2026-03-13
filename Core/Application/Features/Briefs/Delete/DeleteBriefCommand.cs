using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Delete;

public record DeleteBriefCommand(Guid Id) : IRequest<ErrorOr<Unit>>;
