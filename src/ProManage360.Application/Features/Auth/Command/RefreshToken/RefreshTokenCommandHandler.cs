using MediatR;
using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Features.Auth.DTOs;
using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler :IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IDateTime _dateTime;
        public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _dateTime = dateTime;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // ========================================
            // STEP 1: Find Refresh Token
            // ========================================
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            // Separately load user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == refreshToken.UserId, cancellationToken);

            // ========================================
            // STEP 2: Validate Refresh Token
            // ========================================
            if (refreshToken.IsRevoked)
            {
                throw new UnauthorizedAccessException("Refresh token has been revoked. Please login again.");
            }

            if (refreshToken.ExpiresAt < _dateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired. Please login again.");
            }

            // ========================================
            // STEP 3: Validate User Status
            // ========================================

            if (user == null || user.IsDeleted || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is no longer active");
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
            // STEP 5: Generate New Tokens
            // ========================================
            var claims = new Dictionary<string, string>()
            {
                {"sub", user.UserId.ToString()},
                {"tenantId", user.TenantId.ToString()},
                {"email", user.Email},
                {"roles", string.Join(",", userRoles) },
                {"name", user.FullName }
            };

            var newAccessToken = _jwtTokenService.GenerateAccessToken(claims);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // ========================================
            // STEP 6: Revoke Old Refresh Token (Token Rotation)
            // ========================================
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken;

            // ========================================
            // STEP 7: Store New Refresh Token
            // ========================================
            var newRefreshTokenEntity = new Domain.Entities.RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = _dateTime.UtcNow.AddDays(7),
                CreatedAt = _dateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // ========================================
            // STEP 8: Return New Tokens
            // ========================================
            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


    }
}
