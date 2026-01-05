using MediatR;
using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Exceptions;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Features.Auth.DTOs;
using ProManage360.Domain.Entities;
using System.ComponentModel;

namespace ProManage360.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IDateTime _dateTime;
        public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, IDateTime dateTime) 
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _dateTime = dateTime;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // ========================================
            // STEP 1: Find User by Email
            // ========================================
            var user = await _context.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .FirstOrDefaultAsync(x => x.Email == request.Email.ToLower(), cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid Email or Password.");
            }

            // ========================================
            // STEP 2: Verify Password
            // ========================================
            var isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (isValidPassword)
            {
                throw new UnauthorizedAccessException("Invalid Email or Password.");
            }

            // ========================================
            // STEP 3: Get Tenant Information
            // ========================================
            var tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenantId == user.TenantId, cancellationToken);

            if (tenant == null || !tenant.IsActive)
            {
                throw new ForbiddenAccessException("Your organization account is inactive. Please contact support.");
            }

            // Check subscription status
            if (tenant.SubscriptionStatus == Domain.Enums.SubscriptionStatus.Suspended ||
                tenant.SubscriptionStatus == Domain.Enums.SubscriptionStatus.Cancelled)
            {
                throw new ForbiddenAccessException($"Your subscription is {tenant.SubscriptionStatus}. Please contact your administrator.");
            }

            // ========================================
            // STEP 4: Get User Roles
            // ========================================
            var userRoles = await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == user.UserId)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.RoleId,
                    (ur, r) => r.RoleName)
                .ToListAsync(cancellationToken);

            if (!userRoles.Any())
            {
                userRoles.Add("Member"); // Default role if none assigned
            }

            // ========================================
            // STEP 5: Generate JWT Tokens
            // ========================================
            var claims = new Dictionary<string, string>()
            {
                {"sub", user.UserId.ToString()},
                {"tenantId", tenant.TenantId.ToString()},
                {"email", user.Email},
                {"roles", string.Join(",", userRoles) },
                {"name", user.FullName }
            };

            var accessToken = _jwtTokenService.GenerateAccessToken(claims);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // ========================================
            // STEP 6: Store Refresh Token
            // ========================================
            var refreshTokenEntity = new Domain.Entities.RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = _dateTime.UtcNow.AddDays(7),
                CreatedAt = _dateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            // ========================================
            // STEP 7: Update LastLoginAt
            // ========================================
            // Find the tracked user entity to update it
            var trackedUser = await _context.Users.FindAsync(new object[] { user.UserId }, cancellationToken);
            if (trackedUser != null)
            {
                trackedUser.LastLoginAt = _dateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // ========================================
            // STEP 8: Build Response
            // ========================================
            return new LoginResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                TenantId = tenant.TenantId,
                TenantName = tenant.TenantName,
                Subdomain = tenant.Subdomain,
                SubscriptionTier = tenant.SubscriptionTier,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Roles = userRoles
            };

        }
    }
}
