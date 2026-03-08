using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Create;

public record CreateJobPositionCommand(
    string Name,
    string Description,
    decimal HourlyCost) : IRequest<ErrorOr<JobPositionDto>>;
