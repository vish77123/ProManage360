namespace ProManage360.Domain.Enums;

/// <summary>
/// Project status
/// </summary>
public enum ProjectStatus
{
    /// <summary>Project in planning phase</summary>
    Planning = 0,

    /// <summary>Project actively in progress</summary>
    Active = 1,

    /// <summary>Project on hold</summary>
    OnHold = 2,

    /// <summary>Project completed</summary>
    Completed = 3,

    /// <summary>Project cancelled</summary>
    Cancelled = 4
}

/// <summary>
/// Priority levels
/// </summary>
public enum Priority
{
    /// <summary>Low priority</summary>
    Low = 0,

    /// <summary>Medium priority</summary>
    Medium = 1,

    /// <summary>High priority</summary>
    High = 2,

    /// <summary>Critical priority</summary>
    Critical = 3
}