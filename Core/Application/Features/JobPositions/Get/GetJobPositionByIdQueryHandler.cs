using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Get;

public class GetJobPositionByIdQueryHandler : IRequestHandler<GetJobPositionByIdQuery, ErrorOr<JobPositionDto>>
{
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;

    public GetJobPositionByIdQueryHandler(IAsyncRepository<JobPosition> jobPositionRepository)
    {
        _jobPositionRepository = jobPositionRepository;
    }

    public async Task<ErrorOr<JobPositionDto>> Handle(GetJobPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var jobPositionId = new JobPositionId(request.Id);

        if (await _jobPositionRepository.FirstOrDefaultAsync(jp => jp.Id == jobPositionId) is not JobPosition jobPosition)
        {
            return Error.NotFound("JobPosition.NotFound", "Puesto de trabajo no encontrado.");
        }

        return new JobPositionDto(
            jobPosition.Id.Value,
            jobPosition.Name,
            jobPosition.Description,
            jobPosition.HourlyCost
        );
    }
}
