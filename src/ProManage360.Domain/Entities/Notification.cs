namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;
using ProManage360.Domain.Enums;

/// <summary>
/// Notification entity - represents a user notification
/// </summary>
public class Notification : TenantEntity
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public NotificationPriority Priority { get; set; }
}

/// <summary>
/// NotificationSetting entity - user notification preferences
/// </summary>
public class NotificationSetting : BaseEntity
{
    public Guid NotificationSettingId { get; set; }
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool TaskAssignedNotify { get; set; }
    public bool TaskUpdatedNotify { get; set; }
    public bool CommentNotify { get; set; }
    public bool MentionNotify { get; set; }
    public bool DeadlineNotify { get; set; }
    public DateTime? UpdatedAt { get; set; }
}