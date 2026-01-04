using Microsoft.EntityFrameworkCore;
using ProManage360.Domain.Entities;

namespace ProManage360.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Tenant> Tenants { get; }
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<Project> Projects { get; }
        DbSet<TeamMember> TeamMembers { get; }
        DbSet<TaskItem> Tasks { get; }
        DbSet<TaskComment> TaskComments { get; }
        DbSet<TaskAttachment> TaskAttachments { get; }
        DbSet<TaskLabel> TaskLabels { get; }
        DbSet<TaskLabelMapping> TaskLabelMappings { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<NotificationSetting> NotificationSettings { get; }
        DbSet<ActivityLog> ActivityLogs { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<SuperAdmin> SuperAdmins { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
