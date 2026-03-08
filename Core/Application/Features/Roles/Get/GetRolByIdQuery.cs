using Domain.Entities.Roles;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Roles.Get;

public record GetRoleByIdQuery(Guid Id) : IRequest<ErrorOr<RoleDto>>;