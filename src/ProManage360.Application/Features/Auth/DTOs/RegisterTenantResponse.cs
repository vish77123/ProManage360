using ProManage360.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.Auth.DTOs
{
    public class RegisterTenantResponse
    {
        // Tenant Information
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public SubscriptionTier SubscriptionTier { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }

        // User Information
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Authentication Tokens
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        // Trial Information
        public DateTime? TrialEndsAt { get; set; }
        public int TrialDaysRemaining { get; set; }

        // Limits
        public int MaxUsers { get; set; }
        public int MaxProjects { get; set; }
        public int MaxStorageGB { get; set; }
    }
}
