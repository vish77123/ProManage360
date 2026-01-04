namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// User entity - represents a user account
/// </summary>
public class User : SoftDeletableTenantEntity
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}