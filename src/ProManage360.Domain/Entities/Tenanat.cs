namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;
using ProManage360.Domain.Enums;

/// <summary>
/// Tenant entity - represents an organization/company
/// </summary>
public class Tenant : AuditableEntity
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Subscription Management
    public SubscriptionTier SubscriptionTier { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }

    // Tier Limits
    public int MaxUsers { get; set; }
    public int MaxProjects { get; set; }
    public int MaxStorageGB { get; set; }

    // Billing
    public decimal MonthlyPrice { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? SubscriptionStartedAt { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }

    // Enterprise Features
    public bool RequiresApproval { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Contact Information
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? BillingAddress { get; set; }
}