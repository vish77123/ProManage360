using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Common.Models;
using ProManage360.Domain.Entities;
using ProManage360.Domain.Enums;

namespace ProManage360.Application.Features.SuperAdmin.Commands.ApproveTenant;

public class ApproveTenantCommandHandler : IRequestHandler<ApproveTenantCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IEmailService _emailService;
    private readonly ILogger<ApproveTenantCommandHandler> _logger;

    public ApproveTenantCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IDateTime dateTime, IEmailService emailService, ILogger<ApproveTenantCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ApproveTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing tenant approval. TenantId: {TenantId}, ApprovedBy: {SuperAdminId}",
            request.TenantId, _currentUserService.UserId);

        // ============================================
        // STEP 1: Find tenant (ignore filters for super admin)
        // ============================================
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TenantId == request.TenantId);

        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", request.TenantId);
            return Result<bool>.Failure("Tenant Not found.");
        }

        // ============================================
        // STEP 2: Validate tenant is pending approval
        // ============================================
        if (!tenant.RequiresApproval)
        {
            _logger.LogWarning("Tenant does not require approval: {TenantId}", request.TenantId);
            return Result<bool>.Failure("This tenant does not require approval");
        }

        if (tenant.IsApproved)
        {
            _logger.LogWarning("Tenant already approved: {TenantId}", request.TenantId);
            return Result<bool>.Failure("This tenant is already approved");
        }

        // ============================================
        // STEP 3: Update tenant with approval
        // ============================================
        tenant.IsApproved = true;
        tenant.IsActive = true;
        tenant.ApprovedBy = _currentUserService.UserId;
        tenant.ApprovedAt = _dateTime.UtcNow;
        tenant.SubscriptionStatus = SubscriptionStatus.Active;
        tenant.SubscriptionStartedAt = _dateTime.UtcNow;

        // Update limits from approval
        tenant.MonthlyPrice = request.MonthlyPrice;
        tenant.MaxUsers = request.MaxUsers > 0 ? request.MaxUsers : 999999; // 0 = unlimited
        tenant.MaxProjects = request.MaxProjects > 0 ? request.MaxProjects : 999999;
        tenant.MaxStorageGB = request.MaxStorageGB;

        // ============================================
        // STEP 4: Activate admin user
        // ============================================
        var adminUser = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.TenantId == tenant.TenantId, cancellationToken);

        if (adminUser == null)
        {
            _logger.LogError("Admin user not found for tenant: {TenantId}", request.TenantId);
            return Result<bool>.Failure("Admin user not found for this tenant");
        }

        adminUser.IsActive = true;
        adminUser.EmailConfirmed = true;

        // ============================================
        // STEP 5: Create default roles for the tenant
        // ============================================
        var adminRole = new Role
        {
            RoleId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            RoleName = "Admin",
            Description = "Full system access",
            IsSystemRole = true,
            CreatedAt = _dateTime.UtcNow
        };

        var managerRole = new Role
        {
            RoleId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            RoleName = "Manager",
            Description = "Project and team management",
            IsSystemRole = true,
            CreatedAt = _dateTime.UtcNow
        };

        var memberRole = new Role
        {
            RoleId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            RoleName = "Member",
            Description = "Standard team member access",
            IsSystemRole = true,
            CreatedAt = _dateTime.UtcNow
        };

        _context.Roles.Add(adminRole);
        _context.Roles.Add(managerRole);
        _context.Roles.Add(memberRole);

        // ========================================
        // STEP 6: Assign Permissions to Roles
        // ========================================

        // Get all permissions from database (seeded in Infrastructure layer)
        var allPermissions = await _context.Permissions.ToListAsync(cancellationToken);

        // Admin gets ALL permissions
        foreach (var permission in allPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = adminRole.RoleId,
                PermissionId = permission.PermissionId
            });
        }

        // Manager gets project and task permissions (not user management)
        var managerPermissions = allPermissions
            .Where(p => p.Category == "Projects" || p.Category == "Tasks" || p.Category == "Reports")
            .ToList();

        foreach (var permission in managerPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = managerRole.RoleId,
                PermissionId = permission.PermissionId
            });
        }

        // Member gets basic view/edit permissions
        var memberPermissions = allPermissions
            .Where(p => p.PermissionName.Contains("View") || p.PermissionName.Contains("Edit"))
            .Where(p => p.Category == "Tasks")
            .ToList();

        foreach (var permission in memberPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = memberRole.RoleId,
                PermissionId = permission.PermissionId
            });
        }

        // ============================================
        // STEP 7: Assign Admin role to user
        // ============================================
        var userRole = new UserRole
        {
            UserRoleId = Guid.NewGuid(),
            UserId = adminUser.UserId,
            RoleId = adminRole.RoleId,
            AssignedAt = _dateTime.UtcNow,
            AssignedBy = _currentUserService.UserId
        };

        _context.UserRoles.Add(userRole);

        // ============================================
        // STEP 8: Save all changes
        // ============================================
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant approved successfully. TenantId: {TenantId}, Subdomain: {Subdomain}",
            tenant.TenantId, tenant.Subdomain);

        // ============================================
        // STEP 9: Send approval email to customer
        // ============================================
        await _emailService.SendWelcomeEmailAsync(
            adminUser.Email,
            adminUser.FirstName,
            tenant.Subdomain,
            "Enterprise",
            tenant.TrialEndsAt ?? _dateTime.UtcNow.AddDays(60));

        // TODO: Send detailed approval email with:
        // - Login URL
        // - Subscription details
        // - Support contact
        // - Getting started guide

        return Result<bool>.Success(true);


    }
}
