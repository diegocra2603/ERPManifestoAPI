using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.GetAll;

public record GetAllJobPositionsQuery : IRequest<ErrorOr<IReadOnlyList<JobPositionDto>>>;
