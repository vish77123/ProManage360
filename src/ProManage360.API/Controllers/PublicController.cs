using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.DTOs;
using ProManage360.Application.Features.Auth.Command.RegisterTenant;
using ProManage360.Application.Features.Auth.DTOs;

namespace ProManage360.Application.Services
{
    /// <summary>
    /// Public endpoints (no authentication required)
    /// </summary
    [ApiController]
    [Route("api/public")]
    [Produces("application/json")]
    public class PublicController : ControllerBase
    {
        private readonly IMediator _mediator; 

        public PublicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new tenant (self-service)
        /// </summary>
        /// <param name="command">Registration details</param>
        /// <returns>Tenant information with authentication tokens</returns>
        /// <response code="201">Tenant created successfully</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="409">Subdomain or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterTenantResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterTenantCommand command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(
                actionName: nameof(Register),
                value: response
                );
        }
    }

}
