using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProManage360.Application.Features.SuperAdmin.Commands.Login;
using ProManage360.Application.Features.SuperAdmin.Queries.GetPendingTenants;

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
    }
}
