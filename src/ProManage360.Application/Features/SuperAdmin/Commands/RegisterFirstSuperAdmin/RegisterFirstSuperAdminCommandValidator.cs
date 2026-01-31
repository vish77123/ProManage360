using FluentValidation;
using ProManage360.Application.Features.SuperAdmin.Commands.RegisterFirstSuperAdmin;

public class RegisterFirstSuperAdminCommandValidator : AbstractValidator<RegisterFirstSuperAdminCommand>
{
    public RegisterFirstSuperAdminCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Must(email => email.EndsWith("@promanage360.com") || email.EndsWith("@yourcompany.com"))
            .WithMessage("Only company emails are allowed for super admin");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain number")
            .Matches(@"[\W_]").WithMessage("Password must contain special character");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.SetupSecret)
            .NotEmpty().WithMessage("Setup secret is required");
    }
}