using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDataValidator;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Clients.Create;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ErrorOr<ClientDto>>
{
    private readonly IAsyncRepository<Client> _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFiscalDataValidatorService _fiscalDataValidatorService;

    public CreateClientCommandHandler(
        IAsyncRepository<Client> clientRepository,
        IUnitOfWork unitOfWork,
        IFiscalDataValidatorService fiscalDataValidatorService)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _fiscalDataValidatorService = fiscalDataValidatorService;
    }

    public async Task<ErrorOr<ClientDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        string? legalName = request.LegalName;

        // Si se proporciona NIT, validar fiscalmente
        if (!string.IsNullOrWhiteSpace(request.Nit))
        {
            // Verificar duplicado
            if (await _clientRepository.ExistsAsync(c => c.Nit == request.Nit && c.AuditField.IsActive))
                return Error.Validation("Client.NitAlreadyExists", "Ya existe un cliente con ese NIT.");

            // Validar NIT en SAT
            var fiscalResult = await _fiscalDataValidatorService.ValidateFiscalDataAsync(request.Nit);
            if (fiscalResult.IsError)
                return fiscalResult.Errors;

            // Si no se proporcionó nombre legal, usar el del SAT
            if (string.IsNullOrWhiteSpace(legalName))
                legalName = fiscalResult.Value.FiscalName;
        }

        var client = new Client(
            new ClientId(Guid.NewGuid()),
            request.Name,
            legalName,
            string.IsNullOrWhiteSpace(request.Nit) ? null : request.Nit,
            request.Address,
            request.Phone,
            request.Email,
            AuditField.Create()
        );

        _clientRepository.Add(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClientDto(
            client.Id.Value,
            client.Name,
            client.LegalName,
            client.Nit,
            client.Address,
            client.Phone,
            client.Email
        );
    }
}
