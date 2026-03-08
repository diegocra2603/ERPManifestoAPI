namespace Application.Features.Auth.Dtos;

public record RoleInfoDto(
    Guid Id,
    string Name,
    string Description
);
