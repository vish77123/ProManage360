using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.SuperAdmin.Queries.GetPendingTenants
{
    public class GetPendingTenantsQueryHandler : IRequestHandler<GetPendingTenantsQuery, Result<List<PendingTenantDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;
        private readonly ILogger<GetPendingTenantsQueryHandler> _logger;

        public GetPendingTenantsQueryHandler(
            IApplicationDbContext context,
            IDateTime dateTime,
            ILogger<GetPendingTenantsQueryHandler> logger)
        {
            _context = context;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Result<List<PendingTenantDto>>> Handle(GetPendingTenantsQuery request, CancellationToken cancellationToken)
        {
            // ============================================
            // IMPORTANT: We need to IGNORE global query filters
            // because super admin should see ALL tenants
            // ============================================
            var pendingTenants = await _context.Tenants
                .IgnoreQueryFilters()
                .OrderBy(t => t.CreatedAt)
                .Where(t => t.RequiresApproval && !t.IsApproved)
                .Select(t => new
                {
                    Tenant = t,
                    // Get the admin user for this tenant (first user created)
                    AdminUser = _context.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.TenantId == t.TenantId)
                    .OrderBy(u => u.CreatedAt)
                    .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var result = pendingTenants.Select(pt => new PendingTenantDto
            {
                TenantId = pt.Tenant.TenantId,
                TenantName = pt.Tenant.TenantName,
                Subdomain = pt.Tenant.Subdomain,
                ContactEmail = pt.Tenant.ContactEmail ?? string.Empty,
                ContactPhone = pt.Tenant.ContactPhone,
                CompanyWebsite = pt.Tenant.CompanyWebsite,
                ExpectedUsers = pt.Tenant.MaxUsers,
                ExpectedProjects = pt.Tenant.MaxProjects,
                BusinessRequirements = null, // TODO: Add this field to Tenant entity if needed
                RequestedAt = pt.Tenant.CreatedAt,
                DaysPending = (int)(_dateTime.UtcNow - pt.Tenant.CreatedAt).TotalDays,
                AdminEmail = pt.AdminUser?.Email ?? string.Empty,
                AdminName = pt.AdminUser?.FullName ?? string.Empty
            }).ToList();

            _logger.LogInformation("Found {Count} pending tenant requests", result.Count);

            return Result<List<PendingTenantDto>>.Success(result);
        }
    }
}
