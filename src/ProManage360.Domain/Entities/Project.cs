namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;
using ProManage360.Domain.Enums;

/// <summary>
/// Project entity - represents a project within a tenant
/// </summary>
public class Project : SoftDeletableTenantEntity
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ProjectKey { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsArchived { get; set; }
}

/// <summary>
/// TeamMember entity - represents project team membership
/// </summary>
public class TeamMember : BaseEntity
{
    public Guid TeamMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string ProjectRole { get; set; } = "Member"; // Owner, Lead, Member, Viewer
    public DateTime JoinedAt { get; set; }
    public Guid AddedBy { get; set; }
}