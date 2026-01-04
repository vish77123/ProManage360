namespace ProManage360.Domain.Enums;

/// <summary>
/// Notification type
/// </summary>
public enum NotificationType
{
    TaskAssigned = 0,
    TaskUpdated = 1,
    TaskCompleted = 2,
    CommentAdded = 3,
    Mentioned = 4,
    DeadlineApproaching = 5,
    ProjectStatusChanged = 6
}

/// <summary>
/// Notification priority
/// </summary>
public enum NotificationPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}