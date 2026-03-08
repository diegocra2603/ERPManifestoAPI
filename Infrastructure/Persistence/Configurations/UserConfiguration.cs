using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private static readonly Guid AdminUserId = new("41d21bf1-155c-43c3-8858-b5b4d047c6ed");
    private static readonly DateTime SeedCreatedAt = new(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, v => new UserId(v));

        builder.Property(u => u.Name)
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        builder.Property(u => u.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Code).IsUnique();

         builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value) ?? new Email(value))
            .IsRequired()
            .HasMaxLength(Email.MaxLength);

        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(x => x.PhoneNumber)
            .HasConversion(phoneNumber => phoneNumber.Value, value => PhoneNumber.Create(value) ?? new PhoneNumber(value))
            .IsRequired()
            .HasMaxLength(PhoneNumber.MaxLength);
        
        builder.Property(user => user.RoleId)
            .HasConversion(
            id => id.Value,
            value => new RoleId(value));

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

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        SeedAdminUser(builder);
    }

    private static void SeedAdminUser(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new
        {
            Id = new UserId(AdminUserId),
            Email = Email.Create("admin@example.com")!,
            Name = "Admin",
            Code = "ADMIN",
            PhoneNumber = PhoneNumber.Create("1234567890")!,
            RoleId = new RoleId(RoleConstants.Ids.SuperAdmin),
            PasswordHash = "$2a$13$PGGUsmD8GeKalR6Bizj1jO2/aaafW9pFi3D2tzllgRDiVtVwEAmFG",
            BiometricEnabled = false,
            PublicKey = string.Empty,
            IsEmailConfirmed = true,
            IsActive = true,
        });

        builder.OwnsOne(u => u.AuditField).HasData(new
        {
            UserId = new UserId(AdminUserId),
            CreatedAt = SeedCreatedAt,
            UpdatedAt = (DateTime?)null,
            DeletedAt = (DateTime?)null,
            IsActive = true
        });
    }
}
