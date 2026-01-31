using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.DTOs;
using ProManage360.Application.Features.Auth.Command.RegisterTenant;
using ProManage360.Application.Features.Auth.DTOs;
using ProManage360.Application.Features.Tenant.Commands.RequestEnterpriseTenant;

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
        private readonly ILogger<PublicController> _logger;

        public PublicController(IMediator mediator, ILogger<PublicController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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

        [HttpPost("request-enterprise")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestEnterprise([FromBody] RequestEnterpriseTenantCommand command)
        {
            _logger.LogInformation(
                "Enterprise request: Company={CompanyName}, Subdomain={Subdomain}, ExpectedUsers={Users}",
                command.CompanyName, command.Subdomain, command.ExpectedUsers);

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Enterprise request failed for {Company}: {Error}",
                    command.CompanyName, result.Errors);
                return BadRequest(new { Error = result.Errors });
            }

            _logger.LogInformation(
                "Enterprise request submitted: TenantId={TenantId}, awaiting approval",
                result.Data);

            return Accepted(new 
            {
                Message = "Enterprise request submitted successfully. Our sales team will contact you within 24 hours.",
                TenantId = result.Data,
                Status = "Pending Approval",
                NextSteps = new[]
                {
                "Your request is being reviewed by our team",
                "You will receive an email confirmation shortly",
                "A sales representative will contact you within 24 hours",
                "Once approved, you'll receive login credentials"
            }
            });
        }
    }

}
