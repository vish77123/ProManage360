using ProManage360.Domain.Enums;

namespace ProManage360.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Response returned after successful login
    /// </summary>
    public class LoginResponse
    {
        // User Information
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Tenant Information
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public SubscriptionTier SubscriptionTier { get; set; }

        // Authentication Tokens
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        // User Roles (for frontend)
        public List<string> Roles { get; set; } = new();

    }
}
