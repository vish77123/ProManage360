namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// Role entity - represents a user role within a tenant
/// </summary>
public class Role : TenantEntity
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
}