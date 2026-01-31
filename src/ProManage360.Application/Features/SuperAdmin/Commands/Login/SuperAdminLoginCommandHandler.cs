namespace ProManage360.Application.Features.SuperAdmin.Commands.Login;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Common.Models;
using ProManage360.Domain.Entities;

public class SuperAdminLoginCommandHandler
    : IRequestHandler<SuperAdminLoginCommand, Result<SuperAdminLoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<SuperAdminLoginCommandHandler> _logger;

    public SuperAdminLoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime,
        ILogger<SuperAdminLoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<SuperAdminLoginResponse>> Handle(
        SuperAdminLoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Super admin login attempt for email: {Email}", request.Email);

        // ============================================
        // STEP 1: Find Super Admin by email
        // ============================================
        var superAdmin = await _context.SuperAdmins
            .FirstOrDefaultAsync(sa => sa.Email == request.Email.ToLower(), cancellationToken);

        if (superAdmin == null)
        {
            _logger.LogWarning("Super admin not found: {Email}", request.Email);
            return Result<SuperAdminLoginResponse>.Failure("Invalid email or password");
        }

        // ============================================
        // STEP 2: Check if super admin is active
        // ============================================
        if (!superAdmin.IsActive)
        {
            _logger.LogWarning("Super admin account is inactive: {Email}", request.Email);
            return Result<SuperAdminLoginResponse>.Failure("Your account is inactive. Contact system administrator.");
        }

        // ============================================
        // STEP 3: Verify password
        // ============================================
        var passwordValid = _passwordHasher.VerifyPassword(request.Password, superAdmin.PasswordHash);

        if (!passwordValid)
        {
            _logger.LogWarning("Invalid password for super admin: {Email}", request.Email);
            return Result<SuperAdminLoginResponse>.Failure("Invalid email or password");
        }

        // ============================================
        // STEP 4: Generate JWT tokens with SPECIAL claims
        // ============================================
        // IMPORTANT: Super Admin JWT has NO tenantId but has isSuperAdmin claim
        var claims = new Dictionary<string, string>
        {
            { "sub", superAdmin.SuperAdminId.ToString() },
            { "email", superAdmin.Email },
            { "isSuperAdmin", "true" }, // Special claim for super admin
            { "roles", "SuperAdmin" }
            // NOTE: NO tenantId claim! Super admins are platform-scoped
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // ============================================
        // STEP 5: Save refresh token
        // ============================================
        var refreshTokenEntity = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = superAdmin.SuperAdminId, // Reusing UserId field for SuperAdminId
            Token = refreshToken,
            ExpiresAt = _dateTime.UtcNow.AddDays(7),
            CreatedAt = _dateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        // ============================================
        // STEP 6: Update last login timestamp
        // ============================================
        superAdmin.LastLoginAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Super admin login successful: {Email}", request.Email);

        // ============================================
        // STEP 7: Return response
        // ============================================
        var response = new SuperAdminLoginResponse
        {
            SuperAdminId = superAdmin.SuperAdminId,
            Email = superAdmin.Email,
            FullName = superAdmin.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result<SuperAdminLoginResponse>.Success(response);
    }
}