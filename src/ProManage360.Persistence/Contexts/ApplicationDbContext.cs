namespace ProManage360.Persistence;

using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Domain.Common;
using ProManage360.Domain.Entities;
using System;
using System.Reflection;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    // DbSets
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
    public DbSet<TaskLabelMapping> TaskLabelMappings => Set<TaskLabelMapping>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationSetting> NotificationSettings => Set<NotificationSetting>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SuperAdmin> SuperAdmins => Set<SuperAdmin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure global query filters
        ConfigureGlobalQueryFilters(modelBuilder);

        // Seed permissions
        SeedPermissions(modelBuilder);
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Multi-tenancy filters
        modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<Role>().HasQueryFilter(r => r.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<Project>().HasQueryFilter(p => p.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<TaskItem>().HasQueryFilter(t => t.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<Notification>().HasQueryFilter(n => n.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<ActivityLog>().HasQueryFilter(a => a.TenantId == _currentUserService.TenantId);
        modelBuilder.Entity<TaskLabel>().HasQueryFilter(tl => tl.TenantId == _currentUserService.TenantId);

        // Soft delete filters
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);
    }

    private void SeedPermissions(ModelBuilder modelBuilder)
    {
        var seedCreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Permission>().HasData(
            // Project Permissions
            new Permission { PermissionId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionName = "Projects.View", Description = "View projects", Category = "Projects", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("11111111-1111-1111-1111-111111111112"), PermissionName = "Projects.Create", Description = "Create new projects", Category = "Projects", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("11111111-1111-1111-1111-111111111113"), PermissionName = "Projects.Edit", Description = "Edit projects", Category = "Projects", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("11111111-1111-1111-1111-111111111114"), PermissionName = "Projects.Delete", Description = "Delete projects", Category = "Projects", CreatedAt = seedCreatedAt },

            // Task Permissions
            new Permission { PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222221"), PermissionName = "Tasks.View", Description = "View tasks", Category = "Tasks", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222222"), PermissionName = "Tasks.Create", Description = "Create new tasks", Category = "Tasks", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222223"), PermissionName = "Tasks.Edit", Description = "Edit tasks", Category = "Tasks", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222224"), PermissionName = "Tasks.Delete", Description = "Delete tasks", Category = "Tasks", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222225"), PermissionName = "Tasks.Assign", Description = "Assign tasks to users", Category = "Tasks", CreatedAt = seedCreatedAt },

            // User Permissions
            new Permission { PermissionId = Guid.Parse("33333333-3333-3333-3333-333333333331"), PermissionName = "Users.View", Description = "View users", Category = "Users", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("33333333-3333-3333-3333-333333333332"), PermissionName = "Users.Create", Description = "Create new users", Category = "Users", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("33333333-3333-3333-3333-333333333333"), PermissionName = "Users.Edit", Description = "Edit user profiles", Category = "Users", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("33333333-3333-3333-3333-333333333334"), PermissionName = "Users.Delete", Description = "Delete users", Category = "Users", CreatedAt = seedCreatedAt },

            // Admin / Reports Permissions
            new Permission { PermissionId = Guid.Parse("44444444-4444-4444-4444-444444444441"), PermissionName = "Roles.Manage", Description = "Manage roles and permissions", Category = "Admin", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("44444444-4444-4444-4444-444444444442"), PermissionName = "Tenant.Manage", Description = "Manage tenant settings", Category = "Admin", CreatedAt = seedCreatedAt },
            new Permission { PermissionId = Guid.Parse("44444444-4444-4444-4444-444444444443"), PermissionName = "Reports.View", Description = "View analytics and reports", Category = "Reports", CreatedAt = seedCreatedAt }
        );

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Process auditable entities
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = _dateTime.UtcNow;
                entry.Entity.CreatedBy = _currentUserService.UserId != Guid.Empty ? _currentUserService.UserId : null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = _dateTime.UtcNow;
            }
        }

        // Auto-assign TenantId for new tenant-scoped entities
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            {
                if (_currentUserService.TenantId != Guid.Empty)
                {
                    entry.Entity.TenantId = _currentUserService.TenantId;
                }
            }
        }

        // Soft delete interceptor
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = _dateTime.UtcNow;
                entry.Entity.DeletedBy = _currentUserService.UserId != Guid.Empty ? _currentUserService.UserId : null;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
