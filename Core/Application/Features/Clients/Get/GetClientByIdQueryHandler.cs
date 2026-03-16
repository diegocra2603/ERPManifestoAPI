using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Get;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ErrorOr<ClientDto>>
{
    private readonly IAsyncRepository<Client> _clientRepository;

    public GetClientByIdQueryHandler(IAsyncRepository<Client> clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ErrorOr<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(c => c.Id == new ClientId(request.Id) && c.AuditField.IsActive);
        if (client is null)
            return Error.NotFound("Client.NotFound", "Cliente no encontrado.");

        return new ClientDto(client.Id.Value, client.Name, client.LegalName, client.Nit, client.Address, client.Phone, client.Email);
    }
}
