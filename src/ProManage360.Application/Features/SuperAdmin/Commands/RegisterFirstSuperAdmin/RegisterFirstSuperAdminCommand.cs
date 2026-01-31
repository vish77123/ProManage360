namespace ProManage360.Application.Features.SuperAdmin.Commands.RegisterFirstSuperAdmin;

using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Models;
using ProManage360.Domain.Entities;

/// <summary>
/// ONE-TIME COMMAND: Register the first super admin (only works if no super admins exist)
/// </summary>
public class RegisterFirstSuperAdminCommand : IRequest<Result<Guid>>
{
    /// <summary>Super admin email</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Super admin password (min 12 characters for security)</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>First name</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Last name</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Setup secret key (from environment variable)</summary>
    public string SetupSecret { get; set; } = string.Empty;
}