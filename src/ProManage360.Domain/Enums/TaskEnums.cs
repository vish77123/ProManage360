namespace ProManage360.Domain.Enums;

/// <summary>
/// Task status for Kanban workflow
/// </summary>
public enum TaskStatus
{
    /// <summary>Not started</summary>
    Todo = 0,

    /// <summary>In progress</summary>
    InProgress = 1,

    /// <summary>In code review</summary>
    InReview = 2,

    /// <summary>Completed</summary>
    Done = 3,

    /// <summary>Blocked by dependencies</summary>
    Blocked = 4
}

/// <summary>
/// Task type classification
/// </summary>
public enum TaskType
{
    /// <summary>General task</summary>
    Task = 0,

    /// <summary>Bug fix</summary>
    Bug = 1,

    /// <summary>New feature</summary>
    Feature = 2,

    /// <summary>User story</summary>
    Story = 3
}