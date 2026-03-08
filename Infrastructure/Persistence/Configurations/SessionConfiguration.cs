using Domain.Entities.Sessions;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, v => new SessionId(v));

        builder.Property(s => s.UserId)
            .HasConversion(id => id.Value, v => new UserId(v));

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

        builder.HasOne<User>()
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId);
    }
}
