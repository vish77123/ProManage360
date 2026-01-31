using MediatR;
using ProManage360.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.SuperAdmin.Queries.GetPendingTenants
{
    /// <summary>
    /// Query to get all pending tenant requests (for super admin dashboard)
    /// </summary>
    public class GetPendingTenantsQuery : IRequest<Result<List<PendingTenantDto>>>
    {
    }

    /// <summary>
    /// DTO for pending tenant information
    /// </summary>
    public class PendingTenantDto
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? CompanyWebsite { get; set; }
        public int ExpectedUsers { get; set; }
        public int ExpectedProjects { get; set; }
        public string? BusinessRequirements { get; set; }
        public DateTime RequestedAt { get; set; }
        public int DaysPending { get; set; }

        // Admin user info (the person who submitted the request)
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
    }
}
