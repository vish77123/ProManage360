namespace ProManage360.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProManage360.Domain.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(512);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        // Unique constraint for Email + TenantId (excluding soft-deleted users)
        builder.HasIndex(u => new { u.Email, u.TenantId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0"); // SQL Server syntax
                                           // For PostgreSQL: .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(u => u.TenantId);
        builder.HasIndex(u => u.IsDeleted);

        builder.Ignore(u => u.FullName);
    }
}