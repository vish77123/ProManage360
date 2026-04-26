namespace ProManage360.Application.Features.SuperAdmin.Commands.RejectTenant;

using MediatR;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Common.Models;
using ProManage360.Domain.Entities;
using ProManage360.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RejectTenantCommandHandler: IRequestHandler<RejectTenantCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RejectTenantCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IEmailService _emailService;

    public RejectTenantCommandHandler(
        IApplicationDbContext context,
        ILogger<RejectTenantCommandHandler> logger,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        IEmailService emailService
    )
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(RejectTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing tenant rejection. TenantId: {TenantId}, RejectedBy: {SuperAdminId}",
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
        // STEP 3: Update tenant status to rejected
        // ============================================
        tenant.IsActive = false; // Set to inactive on rejection
        // tenant.RejectionReason = request.RejectionReason; // Tenant does not have RejectionReason
        // tenant.RejectedBy = _currentUserService.UserId; // Tenant does not have RejectedBy
        // tenant.RejectedAt = _dateTime.UtcNow; // Tenant does not have RejectedAt
        tenant.SubscriptionStatus = SubscriptionStatus.Cancelled; // Assuming Cancelled based on Enums
        tenant.IsApproved = false;
        tenant.SubscriptionStartedAt = null;

        // Mark all tenant-related records as inactive
        var tenatUsers = await _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.TenantId == tenant.TenantId)
            .ToListAsync(cancellationToken);

        foreach (var user in tenatUsers)
        {
            user.IsActive = false;
        }

        // ============================================
        // STEP 4: Send rejection email
        // ============================================
        try
        {
            string subject = "Registration Rejected - ProManage360";
            string message = $"Dear {tenant.TenantName},\n\n"
                           + $"We regret to inform you that your registration has been rejected.\n\n"
                           + $"Reason: {request.RejectionReason}\n\n"
                           + $"You can contact support for any questions.\n\n"
                           + "Best regards,\nProManage360 Team";

            // Get admin user email to send rejection to
            // Assuming first user is admin or we find one with an Admin role if available in User entity, otherwise first user
            var adminUser = tenatUsers.FirstOrDefault();
            string? adminEmail = adminUser?.Email ?? tenant.ContactEmail;

            if (!string.IsNullOrEmpty(adminEmail))
            {
                // TODO: Update IEmailService to include SendEmailAsync or use a specialized method for rejection
                // await _emailService.SendEmailAsync(adminEmail, subject, message);
                _logger.LogInformation("Rejection email intended to be sent to {Email} but SendEmailAsync is missing from IEmailService", adminEmail);
            }
            else
            {
                _logger.LogWarning("Could not send rejection email: No admin user found for tenant {TenantId}", tenant.TenantId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send rejection email for tenant {TenantId}", tenant.TenantId);
            // Don't fail the whole operation if email fails
        }

        // ============================================
        // STEP 5: Save changes
        // ============================================
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tenant rejected successfully: {TenantId}", request.TenantId);
            return Result<bool>.Success(true); // Fixed: Removed the second parameter to match the signature in Result.cs
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting tenant {TenantId}", request.TenantId);
            return Result<bool>.Failure("Failed to reject tenant");
        }
    }
}

