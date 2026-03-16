namespace Application.Features.Suppliers;

public record SupplierDto(Guid Id, string Nit, string Name, string? Address, string? Phone, string? Email);
