using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.SuperAdmin.Commands.RegisterFirstSuperAdmin
{
    public class RegisterFirstSuperAdminCommandHandler : IRequestHandler<RegisterFirstSuperAdminCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterFirstSuperAdminCommandHandler> _logger;

        public RegisterFirstSuperAdminCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            ILogger<RegisterFirstSuperAdminCommandHandler> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(RegisterFirstSuperAdminCommand request, CancellationToken cancellationToken)
        {
            // ============================================
            // SECURITY CHECK 1: Verify setup secret
            // ============================================
            var expectedSecret = _configuration["Security:SetupSecret"]
                ?? Environment.GetEnvironmentVariable("SETUP_SECRET");

            if (string.IsNullOrEmpty(expectedSecret))
            {
                _logger.LogError("Setup secret not configured in environment");
                return Result<Guid>.Failure("Setup secret not configured. Contact system administrator.");
            }

            if (request.SetupSecret != expectedSecret)
            {
                _logger.LogWarning("Invalid setup secret provided");
                return Result<Guid>.Failure("Invalid setup secret");
            }

            // ============================================
            // SECURITY CHECK 2: Ensure this is FIRST super admin only
            // ============================================
            var superAdminExists = await _context.SuperAdmins.AnyAsync(cancellationToken);

            if (superAdminExists)
            {
                _logger.LogWarning("❌ Attempted to create super admin but one already exists");
                return Result<Guid>.Failure(
                    "A super admin already exists. This endpoint is disabled. " +
                    "Contact existing super admin to create additional admin accounts.");
            }

            // ============================================
            // CREATE FIRST SUPER ADMIN
            // ============================================
            var superAdmin = new Domain.Entities.SuperAdmin
            {
                SuperAdminId = Guid.NewGuid(),
                Email = request.Email.ToLower(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.SuperAdmins.Add(superAdmin);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "✅ First super admin created successfully! Email: {Email}, SuperAdminId: {Id}",
                superAdmin.Email, superAdmin.SuperAdminId);

            return Result<Guid>.Success(superAdmin.SuperAdminId);
        }
    }
}
