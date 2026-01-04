namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// ActivityLog entity - audit trail for all entity changes
/// </summary>
public class ActivityLog : TenantEntity
{
    public Guid ActivityLogId { get; set; }
    public Guid? UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; } // JSON
    public string? NewValue { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}