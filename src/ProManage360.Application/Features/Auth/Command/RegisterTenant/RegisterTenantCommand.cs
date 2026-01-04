using MediatR;
using ProManage360.Application.Features.Auth.DTOs;
using ProManage360.Domain.Enums;

namespace ProManage360.Application.Features.Auth.Command.RegisterTenant
{
    public class RegisterTenantCommand : IRequest<RegisterTenantResponse>
    {
        public string TenantName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;

        // First User (Admin) Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Subscription Tier (Free or Professional)
        public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;

        // Optional Contact Information
        public string? PhoneNumber { get; set; }
        public string? CompanyWebsite { get; set; }

    }
}
