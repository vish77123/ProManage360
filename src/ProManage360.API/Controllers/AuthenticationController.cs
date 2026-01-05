namespace ProManage360.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProManage360.Application.Features.Auth.Command.Login;
using ProManage360.Application.Features.Auth.Command.RefreshToken;
using ProManage360.Application.Features.Auth.DTOs;

/// <summary>
/// Authentication endpoints
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>User information with authentication tokens</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="403">Account suspended or inactive</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="command">Refresh token</param>
    /// <returns>New access token and refresh token</returns>
    /// <response code="200">Tokens refreshed successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Logout (revoke refresh token)
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        // TODO: Implement LogoutCommand to revoke refresh token
        // For now, client can just delete tokens from localStorage
        return Ok(new { message = "Logout successful. Please delete tokens from client." });
    }
}