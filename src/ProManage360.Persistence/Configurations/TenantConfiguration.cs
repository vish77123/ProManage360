namespace ProManage360.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProManage360.Domain.Entities;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.TenantId);

        builder.Property(t => t.TenantName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Subdomain)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.Subdomain).IsUnique();

        builder.Property(t => t.SubscriptionTier)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.SubscriptionStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.MaxUsers).HasDefaultValue(5);
        builder.Property(t => t.MaxProjects).HasDefaultValue(3);
        builder.Property(t => t.MaxStorageGB).HasDefaultValue(1);
        builder.Property(t => t.MonthlyPrice).HasColumnType("decimal(18,2)");
        builder.Property(t => t.IsActive).HasDefaultValue(true);
        builder.Property(t => t.RequiresApproval).HasDefaultValue(false);
        builder.Property(t => t.IsApproved).HasDefaultValue(false);
    }
}