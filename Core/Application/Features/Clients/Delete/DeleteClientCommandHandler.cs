using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Delete;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<Client> _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClientCommandHandler(
        IAsyncRepository<Client> clientRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(c => c.Id == new ClientId(request.Id) && c.AuditField.IsActive);
        if (client is null)
            return Error.NotFound("Client.NotFound", "Cliente no encontrado.");

        client.MarkAsDeleted();
        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
