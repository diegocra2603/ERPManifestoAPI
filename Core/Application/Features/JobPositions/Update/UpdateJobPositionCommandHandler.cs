using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Update;

public class UpdateJobPositionCommandHandler : IRequestHandler<UpdateJobPositionCommand, ErrorOr<JobPositionDto>>
{
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateJobPositionCommandHandler(
        IAsyncRepository<JobPosition> jobPositionRepository,
        IUnitOfWork unitOfWork)
    {
        _jobPositionRepository = jobPositionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<JobPositionDto>> Handle(UpdateJobPositionCommand request, CancellationToken cancellationToken)
    {
        var jobPositionId = new JobPositionId(request.Id);

        if (await _jobPositionRepository.FirstOrDefaultAsync(jp => jp.Id == jobPositionId) is not JobPosition jobPosition)
        {
            return Error.NotFound("JobPosition.NotFound", "Puesto de trabajo no encontrado.");
        }

        if (jobPosition.AuditField.IsDeleted)
        {
            return Error.NotFound("JobPosition.NotFound", "El puesto de trabajo ya fue eliminado.");
        }

        var existingWithName = await _jobPositionRepository.FirstOrDefaultAsync(jp => jp.Name == request.Name);
        if (existingWithName is not null && existingWithName.Id != jobPosition.Id)
        {
            return Error.Validation("JobPosition.NameAlreadyExists", "Ya existe un puesto de trabajo con ese nombre.");
        }

        jobPosition.Update(request.Name, request.Description, request.HourlyCost);

        _jobPositionRepository.Update(jobPosition);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new JobPositionDto(
            jobPosition.Id.Value,
            jobPosition.Name,
            jobPosition.Description,
            jobPosition.HourlyCost
        );
    }
}
