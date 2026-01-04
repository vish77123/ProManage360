namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// Permission entity - global permissions (not tenant-scoped)
/// </summary>
public class Permission : BaseEntity
{
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// RolePermission entity - many-to-many relationship between roles and permissions
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}