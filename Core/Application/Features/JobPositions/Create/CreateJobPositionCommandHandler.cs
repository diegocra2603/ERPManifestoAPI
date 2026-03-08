using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.JobPositions.Create;

public class CreateJobPositionCommandHandler : IRequestHandler<CreateJobPositionCommand, ErrorOr<JobPositionDto>>
{
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobPositionCommandHandler(
        IAsyncRepository<JobPosition> jobPositionRepository,
        IUnitOfWork unitOfWork)
    {
        _jobPositionRepository = jobPositionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<JobPositionDto>> Handle(CreateJobPositionCommand request, CancellationToken cancellationToken)
    {
        if (await _jobPositionRepository.ExistsAsync(jp => jp.Name == request.Name))
        {
            return Error.Validation("JobPosition.NameAlreadyExists", "Ya existe un puesto de trabajo con ese nombre.");
        }

        var jobPosition = new JobPosition(
            new JobPositionId(Guid.NewGuid()),
            request.Name,
            request.Description,
            request.HourlyCost,
            AuditField.Create()
        );

        _jobPositionRepository.Add(jobPosition);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new JobPositionDto(
            jobPosition.Id.Value,
            jobPosition.Name,
            jobPosition.Description,
            jobPosition.HourlyCost
        );
    }
}
