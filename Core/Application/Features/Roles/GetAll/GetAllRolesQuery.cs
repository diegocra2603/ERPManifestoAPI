using Domain.Entities.Roles;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Roles.GetAll;

public record GetAllRolesQuery : IRequest<ErrorOr<IReadOnlyList<RoleDto>>>;