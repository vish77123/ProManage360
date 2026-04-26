namespace ProManage360.Application.Features.SuperAdmin.Commands.ApproveTenant;

using MediatR;
using ProManage360.Application.Common.Models;

/// <summary>
/// Command to approve a pending enterprise tenant
/// </summary>
public class ApproveTenantCommand : IRequest<Result<bool>>
{
    /// <summary>Tenant ID to approve</summary>
    public Guid TenantId { get; set; }

    /// <summary>Monthly subscription price (custom for enterprise)</summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>Maximum users allowed (0 = unlimited)</summary>
    public int MaxUsers { get; set; }

    /// <summary>Maximum projects allowed (0 = unlimited)</summary>
    public int MaxProjects { get; set; }

    /// <summary>Maximum storage in GB</summary>
    public int MaxStorageGB { get; set; }

    /// <summary>Optional approval notes</summary>
    public string? ApprovalNotes { get; set; }
}