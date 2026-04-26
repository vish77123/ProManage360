namespace ProManage360.Application.Features.SuperAdmin.Commands.RejectTenant;

using FluentValidation;

public class RejectTenantCommandValidator : AbstractValidator<RejectTenantCommand>
{
    public RejectTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters.");
    }
}