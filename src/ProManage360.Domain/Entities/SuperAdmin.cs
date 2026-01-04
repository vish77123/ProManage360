namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// SuperAdmin entity - platform administrators (not tenant-scoped)
/// </summary>
public class SuperAdmin : BaseEntity
{
    public Guid SuperAdminId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
