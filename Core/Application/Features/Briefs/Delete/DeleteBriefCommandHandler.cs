using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Briefs;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Briefs.Delete;

public class DeleteBriefCommandHandler : IRequestHandler<DeleteBriefCommand, ErrorOr<Unit>>
{
    private readonly IAsyncRepository<Brief> _briefRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBriefCommandHandler(
        IAsyncRepository<Brief> briefRepository,
        IUnitOfWork unitOfWork)
    {
        _briefRepository = briefRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteBriefCommand request, CancellationToken cancellationToken)
    {
        var briefId = new BriefId(request.Id);

        if (await _briefRepository.FirstOrDefaultAsync(b => b.Id == briefId) is not Brief brief)
        {
            return Error.NotFound("Brief.NotFound", "Brief no encontrado.");
        }

        if (brief.AuditField.IsDeleted)
        {
            return Error.Validation("Brief.AlreadyDeleted", "El brief ya fue eliminado.");
        }

        brief.MarkAsDeleted();

        _briefRepository.Update(brief);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
