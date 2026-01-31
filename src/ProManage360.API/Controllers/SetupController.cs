// ============================================
// WebAPI/Controllers/SetupController.cs
// ============================================

namespace ProManage360.WebAPI.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProManage360.Application.Features.SuperAdmin.Commands.RegisterFirstSuperAdmin;

/// <summary>
/// One-time setup endpoints (disabled after first use)
/// </summary>
[ApiController]
[Route("api/setup")]
[AllowAnonymous]
public class SetupController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SetupController> _logger;

    public SetupController(IMediator mediator, ILogger<SetupController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// ONE-TIME ONLY: Register the first super admin
    /// Requires SETUP_SECRET environment variable
    /// This endpoint is automatically disabled after first super admin is created
    /// </summary>
    /// <remarks>
    /// Example request:
    /// 
    ///     POST /api/setup/first-admin
    ///     {
    ///       "email": "admin@promanage360.com",
    ///       "password": "SuperSecurePassword123!",
    ///       "firstName": "John",
    ///       "lastName": "Doe",
    ///       "setupSecret": "your-secret-from-env"
    ///     }
    /// 
    /// </remarks>
    [HttpPost("first-admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterFirstSuperAdmin([FromBody] RegisterFirstSuperAdminCommand command)
    {
        _logger.LogWarning("🔐 First super admin registration endpoint called");

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(new { error = result.Errors });

        _logger.LogInformation("✅ First super admin registration successful");

        return Ok(result.Data);
    }
}