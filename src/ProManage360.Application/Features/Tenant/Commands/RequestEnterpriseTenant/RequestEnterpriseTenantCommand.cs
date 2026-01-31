namespace ProManage360.Application.Features.Tenant.Commands.RequestEnterpriseTenant;

using MediatR;
using ProManage360.Application.Common.Models;

/// <summary>
/// Command to request an Enterprise tenant (requires super admin approval)
/// </summary>
public class RequestEnterpriseTenantCommand : IRequest<Result<Guid>>
{
    /// <summary>Company/Organization name</summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>Preferred subdomain (e.g., "acme" for acme.promanage360.com)</summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>Admin user's first name</summary>
    public string AdminFirstName { get; set; } = string.Empty;

    /// <summary>Admin user's last name</summary>
    public string AdminLastName { get; set; } = string.Empty;

    /// <summary>Admin email (will be used for login)</summary>
    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>Temporary password (will be hashed)</summary>
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>Company size (number of employees)</summary>
    public int CompanySize { get; set; }

    /// <summary>Expected number of users</summary>
    public int ExpectedUsers { get; set; }

    /// <summary>Expected number of projects</summary>
    public int ExpectedProjects { get; set; }

    /// <summary>Business requirements/notes</summary>
    public string? BusinessRequirements { get; set; }

    /// <summary>Industry/sector</summary>
    public string? Industry { get; set; }

    /// <summary>Contact phone number</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Company website</summary>
    public string? CompanyWebsite { get; set; }
}