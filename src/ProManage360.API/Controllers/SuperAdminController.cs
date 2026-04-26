using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProManage360.Application.Features.SuperAdmin.Commands.ApproveTenant;
using ProManage360.Application.Features.SuperAdmin.Commands.Login;
using ProManage360.Application.Features.SuperAdmin.Queries.GetPendingTenants;
using ProManage360.Application.Features.SuperAdmin.Commands.RejectTenant;
using ProManage360.Domain.Entities;

namespace ProManage360.API.Controllers
{
    [ApiController]
    [Route("api/Admin")]
    [Authorize]
    public class SuperAdminController : ControllerBase
    {
        public readonly IMediator mediator;
        public readonly ILogger<SuperAdminController> logger;

        public SuperAdminController(IMediator mediator, ILogger<SuperAdminController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/SuperAdmin-Login")]
        public async Task<IActionResult> SuperAdminLogin(SuperAdminLoginCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.Succeeded)
            {
                return Unauthorized(new { error = response.Errors });
            }
            return Ok(response.Data);
        }

        [HttpGet("tenants/pending")]
        [ProducesResponseType(typeof(List<PendingTenantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPendingTenants()
        {
            var result = await mediator.Send(new GetPendingTenantsQuery());

            if (!result.Succeeded)
                return BadRequest(new {error = result.Errors});

            return Ok(result.Data);
        }

        [HttpPost("tenanta/{tenantId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveTenant(Guid tenantId, [FromBody] ApproveTenantCommand command)
        {
            // Ensure route parameter matches body
            command.TenantId = tenantId;

            var result = await mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new
            {
                message = "Tenant approved successfully",
                tenantId = tenantId
            });
        }

        [HttpPost("tenanta/{tenantId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RejectTenant(Guid tenantId, [FromBody] RejectTenantCommand command)
        {
            // Ensure route parameter matches body
            command.TenantId = tenantId;

            var result = await mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new
            {
                message = "Tenant rejected successfully",
                tenantId = tenantId
            });
        }
    }
}
