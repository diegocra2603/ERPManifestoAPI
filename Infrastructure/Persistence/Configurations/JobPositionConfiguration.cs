using Domain.Entities.JobPositions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.HasKey(jp => jp.Id);

        builder.Property(jp => jp.Id)
            .HasConversion(id => id.Value, v => new JobPositionId(v));

        builder.Property(jp => jp.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(jp => jp.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(jp => jp.HourlyCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

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
