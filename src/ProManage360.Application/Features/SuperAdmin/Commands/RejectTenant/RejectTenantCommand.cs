namespace ProManage360.Application.Features.SuperAdmin.Commands.RejectTenant;

using MediatR;
using ProManage360.Application.Common.Models;

public class RejectTenantCommand : IRequest<Result<bool>>
{
    public Guid TenantId { get; set; }
    public string? RejectionReason { get; set; }
}