namespace ProManage360.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProManage360.Domain.Entities;

public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.TaskId);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Description)
            .HasColumnType("text");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.TaskType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.EstimatedHours)
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.ActualHours)
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.KanbanOrder)
            .HasDefaultValue(0);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(t => new { t.ProjectId, t.TaskNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.AssignedToId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.IsDeleted);

        builder.Ignore(t => t.TaskKey);
    }
}