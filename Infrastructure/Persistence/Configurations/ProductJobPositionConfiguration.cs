using Domain.Entities.JobPositions;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class ProductJobPositionConfiguration : IEntityTypeConfiguration<ProductJobPosition>
{
    public void Configure(EntityTypeBuilder<ProductJobPosition> builder)
    {
        builder.HasKey(pjp => pjp.Id);

        builder.Property(pjp => pjp.Id)
            .HasConversion(id => id.Value, v => new ProductJobPositionId(v));

        builder.Property(pjp => pjp.ProductId)
            .HasConversion(id => id.Value, v => new ProductId(v))
            .IsRequired();

        builder.Property(pjp => pjp.JobPositionId)
            .HasConversion(id => id.Value, v => new JobPositionId(v))
            .IsRequired();

        builder.Property(pjp => pjp.Hours)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(pjp => pjp.JobPosition)
            .WithMany()
            .HasForeignKey(pjp => pjp.JobPositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
