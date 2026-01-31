using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Common.Models;
using ProManage360.Domain.Entities;
using ProManage360.Domain.Enums;


namespace ProManage360.Application.Features.Tenant.Commands.RequestEnterpriseTenant
{
    public class RequestEnterpriseTenantCommandHandler : IRequestHandler<RequestEnterpriseTenantCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDateTime _dateTime;
        private readonly ILogger<RequestEnterpriseTenantCommandHandler> _logger;

        public RequestEnterpriseTenantCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IDateTime dateTime, ILogger<RequestEnterpriseTenantCommandHandler> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(RequestEnterpriseTenantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
            "Processing enterprise tenant request for company: {CompanyName}, subdomain: {Subdomain}",
            request.CompanyName, request.Subdomain);

            // ============================================
            // STEP 1: Check if subdomain is already taken
            // ============================================
            var isSubDomainExists = await _context.Tenants
                .AnyAsync(x => x.Subdomain == request.Subdomain.ToLower(), cancellationToken);

            if (isSubDomainExists)
            {
                _logger.LogWarning("Subdomain already taken: {Subdomain}", request.Subdomain);
                return Result<Guid>.Failure($"Subdomain '{request.Subdomain}' is already taken");
            }

            // ============================================
            // STEP 2: Check if admin email is already used
            // ============================================
            var emailExists = await _context.Users
                .AnyAsync(x => x.Email == request.AdminEmail.ToLower(), cancellationToken);

            if (emailExists)
            {
                _logger.LogWarning("Email already registered: {Email}", request.AdminEmail);
                return Result<Guid>.Failure($"Email '{request.AdminEmail}' is already registered");
            }

            // ============================================
            // STEP 3: Create Pending Tenant (NOT YET APPROVED)
            // ============================================
            var tenant = new Domain.Entities.Tenant
            {
                TenantId = Guid.NewGuid(),
                TenantName = request.CompanyName,
                Subdomain = request.Subdomain.ToLower(),

                // Enterprise Tier Configuration
                SubscriptionTier = SubscriptionTier.Enterprise,
                SubscriptionStatus = SubscriptionStatus.Trial, // Starts as trial until approved

                // Enterprise gets unlimited resources (set high limits)
                MaxUsers = 10000,
                MaxProjects = 10000,
                MaxStorageGB = 1000, // 1TB

                // Pricing (to be negotiated by sales)
                MonthlyPrice = 0, // Will be set during approval

                // IMPORTANT: Not approved yet, waiting for super admin
                IsActive = false,
                RequiresApproval = true,
                IsApproved = false,

                // Trial period (60 days for enterprise)
                TrialEndsAt = _dateTime.UtcNow.AddDays(60),

                // Contact Information
                ContactEmail = request.AdminEmail,
                ContactPhone = request.ContactPhone,
                CompanyWebsite = request.CompanyWebsite,

                // Audit fields (will be set by SaveChangesAsync interceptor)
                CreatedAt = _dateTime.UtcNow
            };

            _context.Tenants.Add(tenant);

            // ============================================
            // STEP 4: Create Admin User (but inactive until approval)
            // ============================================
            var adminUser = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                Email = request.AdminEmail.ToLower(),
                PasswordHash = _passwordHasher.HashPassword(request.AdminPassword),
                FirstName = request.AdminFirstName,
                LastName = request.AdminLastName,

                // User is inactive until tenant is approved
                IsActive = false,
                EmailConfirmed = false,

                IsDeleted = false,
                CreatedAt = _dateTime.UtcNow
            };

            _context.Users.Add(adminUser);

            // ============================================
            // STEP 5: Save to database
            // ============================================
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Enterprise tenant request created successfully. TenantId: {TenantId}, awaiting approval",
                tenant.TenantId);

            // ============================================
            // STEP 6: Notify Sales Team (TODO: Implement email)
            // ============================================
            // TODO: Send email to sales@promanage360.com with request details
            // - Company Name
            // - Contact Email
            // - Expected Users/Projects
            // - Business Requirements
            // - Link to admin approval portal

            return Result<Guid>.Success(tenant.TenantId);
        }
    }
}
