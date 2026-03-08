using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Get;

public record GetJobPositionByIdQuery(Guid Id) : IRequest<ErrorOr<JobPositionDto>>;
