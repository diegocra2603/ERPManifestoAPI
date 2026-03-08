using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.GetAll;

public class GetAllJobPositionsQueryHandler : IRequestHandler<GetAllJobPositionsQuery, ErrorOr<IReadOnlyList<JobPositionDto>>>
{
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;

    public GetAllJobPositionsQueryHandler(IAsyncRepository<JobPosition> jobPositionRepository)
    {
        _jobPositionRepository = jobPositionRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<JobPositionDto>>> Handle(GetAllJobPositionsQuery request, CancellationToken cancellationToken)
    {
        var jobPositions = await _jobPositionRepository.GetAsync(jp => jp.AuditField.IsActive);

        return jobPositions
            .Select(jp => new JobPositionDto(jp.Id.Value, jp.Name, jp.Description, jp.HourlyCost))
            .ToList()
            .AsReadOnly();
    }
}
