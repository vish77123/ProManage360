namespace ProManage360.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProManage360.Domain.Entities;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(256);

        builder.Property(r => r.IsSystemRole)
            .HasDefaultValue(false);

        builder.HasIndex(r => new { r.RoleName, r.TenantId }).IsUnique();
        builder.HasIndex(r => r.TenantId);
    }
}