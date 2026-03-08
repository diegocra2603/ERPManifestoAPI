using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.JobPositions;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JobPositions.Delete;

public class DeleteJobPositionCommandHandler : IRequestHandler<DeleteJobPositionCommand, ErrorOr<Unit>>
{
    private readonly IAsyncRepository<JobPosition> _jobPositionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJobPositionCommandHandler(
        IAsyncRepository<JobPosition> jobPositionRepository,
        IUnitOfWork unitOfWork)
    {
        _jobPositionRepository = jobPositionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteJobPositionCommand request, CancellationToken cancellationToken)
    {
        var jobPositionId = new JobPositionId(request.Id);

        if (await _jobPositionRepository.FirstOrDefaultAsync(jp => jp.Id == jobPositionId) is not JobPosition jobPosition)
        {
            return Error.NotFound("JobPosition.NotFound", "Puesto de trabajo no encontrado.");
        }

        if (jobPosition.AuditField.IsDeleted)
        {
            return Error.Validation("JobPosition.AlreadyDeleted", "El puesto de trabajo ya fue eliminado.");
        }

        jobPosition.MarkAsDeleted();

        _jobPositionRepository.Update(jobPosition);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
