using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Update;

public record UpdateJobPositionCommand(
    Guid Id,
    string Name,
    string Description,
    decimal HourlyCost) : IRequest<ErrorOr<JobPositionDto>>;
