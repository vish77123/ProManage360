using FluentValidation;

namespace ProManage360.Application.Features.SuperAdmin.Commands.ApproveTenant;

public class ApproveTenantCommandValidator : AbstractValidator<ApproveTenantCommand>
{
    public ApproveTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.MonthlyPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly price must be positive or zero")
            .LessThan(1000000).WithMessage("Monthly price seems unrealistic");

        RuleFor(x => x.MaxUsers)
            .GreaterThanOrEqualTo(0).WithMessage("Max users must be 0 (unlimited) or positive");

        RuleFor(x => x.MaxProjects)
            .GreaterThanOrEqualTo(0).WithMessage("Max projects must be 0 (unlimited) or positive");

        RuleFor(x => x.MaxStorageGB)
            .GreaterThan(0).WithMessage("Max storage must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Max storage seems unrealistic (max 10TB)");

        RuleFor(x => x.ApprovalNotes)
            .MaximumLength(1000).WithMessage("Approval notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.ApprovalNotes));
    }

}
