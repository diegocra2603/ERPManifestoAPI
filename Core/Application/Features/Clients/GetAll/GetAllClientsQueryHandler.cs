using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.GetAll;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, ErrorOr<IReadOnlyList<ClientDto>>>
{
    private readonly IAsyncRepository<Client> _clientRepository;

    public GetAllClientsQueryHandler(IAsyncRepository<Client> clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<ClientDto>>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientRepository.GetAsync(c => c.AuditField.IsActive);

        return clients
            .Select(c => new ClientDto(c.Id.Value, c.Name, c.LegalName, c.Nit, c.Address, c.Phone, c.Email))
            .ToList()
            .AsReadOnly();
    }
}
