using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Update;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ErrorOr<ClientDto>>
{
    private readonly IAsyncRepository<Client> _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientCommandHandler(
        IAsyncRepository<Client> clientRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ClientDto>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(c => c.Id == new ClientId(request.Id) && c.AuditField.IsActive);
        if (client is null)
            return Error.NotFound("Client.NotFound", "Cliente no encontrado.");

        if (!string.IsNullOrWhiteSpace(request.Nit) &&
            await _clientRepository.ExistsAsync(c => c.Nit == request.Nit && c.Id != new ClientId(request.Id) && c.AuditField.IsActive))
            return Error.Validation("Client.NitAlreadyExists", "Ya existe otro cliente con ese NIT.");

        client.Update(
            request.Name,
            request.LegalName,
            string.IsNullOrWhiteSpace(request.Nit) ? null : request.Nit,
            request.Address,
            request.Phone,
            request.Email);

        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClientDto(client.Id.Value, client.Name, client.LegalName, client.Nit, client.Address, client.Phone, client.Email);
    }
}
