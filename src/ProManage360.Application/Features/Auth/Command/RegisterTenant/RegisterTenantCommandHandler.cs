using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Features.Auth.DTOs;
using ProManage360.Domain.Entities;
using ProManage360.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.Auth.Command.RegisterTenant
{
    public class RegisterTenantCommandHandler : IRequestHandler<RegisterTenantCommand, RegisterTenantResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;
        private readonly IDateTime _dateTime;

        public RegisterTenantCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, IEmailService emailService, IDateTime dateTime)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
            _dateTime = dateTime;
        }

        public async Task<RegisterTenantResponse> Handle(RegisterTenantCommand request, CancellationToken cancellationToken)
        {
            // Step 1 : Create Tenant.
            var tenant = new Tenant
            {
                TenantId = Guid.NewGuid(),
                TenantName = request.TenantName,
                Subdomain = request.Subdomain.ToLower(), // Always lowercase
                IsActive = true,

                // Subscription Setup
                SubscriptionTier = request.Tier,
                SubscriptionStatus = SubscriptionStatus.Trial, // Always start with trial
                SubscriptionStartedAt = _dateTime.UtcNow,
                TrialEndsAt = _dateTime.UtcNow.AddDays(30), // 30-day trial

                // Set tier limits based on subscription
                MaxUsers = request.Tier == SubscriptionTier.Free ? 5 : 25,
                MaxProjects = request.Tier == SubscriptionTier.Free ? 3 : 50,
                MaxStorageGB = request.Tier == SubscriptionTier.Free ? 1 : 10,
                MonthlyPrice = request.Tier == SubscriptionTier.Free ? 0 : 29.99m,

                // Self-service registration doesn't require approval
                RequiresApproval = false,
                IsApproved = true,

                // Contact Information (Optional)
                ContactEmail = request.Email,
                ContactPhone = request.PhoneNumber,
                CompanyWebsite = request.CompanyWebsite,

                // Audit fields (set automatically by SaveChangesAsync interceptor)
                CreatedAt = _dateTime.UtcNow
            };

            _context.Tenants.Add(tenant);

            // ========================================
            // STEP 2: Create Default Roles
            // ========================================

            // Admin Role - Full access
            var adminRole = new Role
            {
                RoleId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                RoleName = "Admin",
                Description = "Full system access",
                IsSystemRole = true, // Cannot be deleted
                CreatedAt = _dateTime.UtcNow
            };

            // Manager Role - Project management access
            var managerRole = new Role
            {
                RoleId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                RoleName = "Manager",
                Description = "Can manage projects and teams",
                IsSystemRole = true,
                CreatedAt = _dateTime.UtcNow
            };

            // Member Role - Basic access
            var memberRole = new Role
            {
                RoleId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                RoleName = "Member",
                Description = "Can view and update assigned tasks",
                IsSystemRole = true,
                CreatedAt = _dateTime.UtcNow
            };

            _context.Roles.Add(adminRole);
            _context.Roles.Add(managerRole);
            _context.Roles.Add(memberRole);

            // ========================================
            // STEP 3: Assign Permissions to Roles
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

            // ========================================
            // STEP 4: Create First User (Admin)
            // ========================================

            var user = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                Email = request.Email.ToLower(), // Normalize email
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = true, // Auto-confirm for now (can add email verification later)
                CreatedAt = _dateTime.UtcNow
            };

            _context.Users.Add(user);

            // ========================================
            // STEP 5: Assign Admin Role to User
            // ========================================

            var userRole = new UserRole
            {
                UserRoleId = Guid.NewGuid(),
                UserId = user.UserId,
                RoleId = adminRole.RoleId,
                AssignedAt = _dateTime.UtcNow,
                AssignedBy = user.UserId // Self-assigned (first user)
            };

            _context.UserRoles.Add(userRole);


            // ========================================
            // STEP 6: Create Notification Settings
            // ========================================

            var notificationSettings = new NotificationSetting
            {
                NotificationSettingId = Guid.NewGuid(),
                UserId = user.UserId,
                EmailNotifications = true,
                PushNotifications = true,
                TaskAssignedNotify = true,
                TaskUpdatedNotify = true,
                CommentNotify = true,
                MentionNotify = true,
                DeadlineNotify = true
            };

            _context.NotificationSettings.Add(notificationSettings);

            // ========================================
            // STEP 7: Save All Changes
            // ========================================

            await _context.SaveChangesAsync(cancellationToken);

            // ========================================
            // STEP 8: Generate JWT Tokens
            // ========================================

            var claims = new Dictionary<string, string>
                {
                    { "sub", user.UserId.ToString() },           // Subject (user ID)
                    { "tenantId", tenant.TenantId.ToString() },  // Tenant ID for multi-tenancy
                    { "email", user.Email },                      // User email
                    { "roles", "Admin" },                         // User roles (comma-separated)
                    { "name", user.FullName }                     // Display name
                };

            var accessToken = _jwtTokenService.GenerateAccessToken(claims);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // ========================================
            // STEP 9: Store Refresh Token
            // ========================================

            var refreshTokenEntity = new Domain.Entities.RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = _dateTime.UtcNow.AddDays(7), // 7 days validity
                CreatedAt = _dateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // ========================================
            // STEP 10: Send Welcome Email (Background)
            // ========================================

            // Fire-and-forget (don't await - email sending shouldn't block response)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendWelcomeEmailAsync(
                        toEmail: user.Email,
                        firstName: user.FirstName,
                        subdomain: tenant.Subdomain,
                        tier: tenant.SubscriptionTier.ToString(),
                        trialEndsAt: tenant.TrialEndsAt!.Value
                    );
                }
                catch (Exception)
                {
                    // Log error but don't fail registration
                    // In production, use ILogger to log this
                }
            }, cancellationToken);

            // ========================================
            // STEP 11: Build Response
            // ========================================

            var response = new RegisterTenantResponse
            {
                TenantId = tenant.TenantId,
                TenantName = tenant.TenantName,
                Subdomain = tenant.Subdomain,
                SubscriptionTier = tenant.SubscriptionTier,
                SubscriptionStatus = tenant.SubscriptionStatus,

                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,

                AccessToken = accessToken,
                RefreshToken = refreshToken,

                TrialEndsAt = tenant.TrialEndsAt,
                TrialDaysRemaining = (int)(tenant.TrialEndsAt!.Value - _dateTime.UtcNow).TotalDays,

                MaxUsers = tenant.MaxUsers,
                MaxProjects = tenant.MaxProjects,
                MaxStorageGB = tenant.MaxStorageGB
            };

            return response;

        }
    }
}
