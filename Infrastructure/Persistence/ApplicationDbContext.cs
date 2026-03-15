using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.FiscalData;
using Domain.Entities.FiscalDocuments;
using Domain.Entities.Products;
using Domain.Entities.SystemSettings;
using Domain.Entities.JobPositions;
using Domain.Entities.Permissions;
using Domain.Entities.Roles;
using Domain.Entities.Sessions;
using Domain.Entities.Users;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IPublisher? _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher? publisher = null) : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<JobPosition> JobPositions { get; set; }
    public DbSet<FiscalDataEntry> FiscalDataEntries { get; set; }
    public DbSet<FiscalDocument> FiscalDocuments { get; set; }
    public DbSet<FiscalDocumentItem> FiscalDocumentItems { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductJobPosition> ProductJobPositions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.GetDomainEvents().Any())
            .SelectMany(e => e.GetDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        if (_publisher is not null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }

        return result;
    }
}
