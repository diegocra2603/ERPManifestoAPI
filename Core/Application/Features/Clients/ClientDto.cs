namespace Application.Features.Clients;

public record ClientDto(Guid Id, string Name, string? LegalName, string? Nit, string? Address, string? Phone, string? Email);
