using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Delete;

public record DeleteJobPositionCommand(Guid Id) : IRequest<ErrorOr<Unit>>;
