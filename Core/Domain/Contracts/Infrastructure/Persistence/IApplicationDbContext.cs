using Domain.Entities.JobPositions;
using Domain.Entities.Permissions;
using Domain.Entities.Roles;
using Domain.Entities.Sessions;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain.Contracts.Infrastructure.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<RolePermission> RolePermissions { get; set; }
    DbSet<Permission> Permissions { get; set; }
    DbSet<Session> Sessions { get; set; }
    DbSet<JobPosition> JobPositions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}