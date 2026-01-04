namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// UserRole entity - many-to-many relationship between users and roles
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserRoleId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
}