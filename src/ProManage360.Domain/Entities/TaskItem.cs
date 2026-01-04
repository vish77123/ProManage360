namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;
using ProManage360.Domain.Enums;

/// <summary>
/// Task entity - represents a task/issue
/// </summary>
public class TaskItem : SoftDeletableTenantEntity
{
    public Guid TaskId { get; set; }
    public Guid ProjectId { get; set; }
    public int TaskNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public TaskType TaskType { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid ReporterId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public int? StoryPoints { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int KanbanOrder { get; set; }

    // Computed property
    public string TaskKey => $"TASK-{TaskNumber}";
}

/// <summary>
/// TaskComment entity - represents a comment on a task
/// </summary>
public class TaskComment : BaseEntity
{
    public Guid TaskCommentId { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// TaskAttachment entity - represents a file attachment on a task
/// </summary>
public class TaskAttachment : BaseEntity
{
    public Guid TaskAttachmentId { get; set; }
    public Guid TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// TaskLabel entity - represents a label/tag for tasks
/// </summary>
public class TaskLabel : TenantEntity
{
    public Guid TaskLabelId { get; set; }
    public Guid? ProjectId { get; set; }
    public string LabelName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#000000";
}

/// <summary>
/// TaskLabelMapping entity - many-to-many relationship between tasks and labels
/// </summary>
public class TaskLabelMapping : BaseEntity
{
    public Guid TaskLabelMappingId { get; set; }
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
}