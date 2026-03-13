using Domain.Entities.Briefs;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class BriefConfiguration : IEntityTypeConfiguration<Brief>
{
    public void Configure(EntityTypeBuilder<Brief> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasConversion(id => id.Value, v => new BriefId(v));

        builder.Property(b => b.ProductId)
            .HasConversion(id => id.Value, v => new ProductId(v))
            .IsRequired();

        builder.Property(b => b.ClientName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(b => b.ContactName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(b => b.Date)
            .IsRequired();

        builder.Property(b => b.CompanyBackground)
            .HasMaxLength(5000);

        builder.Property(b => b.BrandingElementsToPreserve)
            .HasMaxLength(5000);

        builder.Property(b => b.CommunicationProblems)
            .HasMaxLength(5000);

        builder.Property(b => b.BrandPerception)
            .HasMaxLength(5000);

        builder.Property(b => b.StartDate);

        builder.Property(b => b.DeliveryDate);

        builder.Property(b => b.DurationMonths);

        builder.Property(b => b.Budget)
            .HasPrecision(18, 2);

        builder.HasMany(b => b.Items)
            .WithOne(i => i.Brief)
            .HasForeignKey(i => i.BriefId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(t => t.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .IsRequired();

            audit.Property(a => a.UpdatedAt);

            audit.Property(a => a.DeletedAt);

            audit.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });
    }
}
