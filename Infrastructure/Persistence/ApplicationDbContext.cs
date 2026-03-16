using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
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
using Persistence.Seed;

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

    // Accounting
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<AccountCatalog> AccountCatalogs { get; set; }
    public DbSet<AccountingPeriod> AccountingPeriods { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
    public DbSet<TaxConfiguration> TaxConfigurations { get; set; }
    public DbSet<TaxTransaction> TaxTransactions { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.SeedAccountingData();
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
