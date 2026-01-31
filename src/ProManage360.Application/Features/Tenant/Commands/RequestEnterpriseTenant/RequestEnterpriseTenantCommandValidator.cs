namespace ProManage360.Application.Features.Tenants.Commands.RequestEnterpriseTenant;

using FluentValidation;
using ProManage360.Application.Features.Tenant.Commands.RequestEnterpriseTenant;
using System.Text.RegularExpressions;

public class RequestEnterpriseTenantCommandValidator : AbstractValidator<RequestEnterpriseTenantCommand>
{
    public RequestEnterpriseTenantCommandValidator()
    {
        // Company Name
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters");

        // Subdomain
        RuleFor(x => x.Subdomain)
            .NotEmpty().WithMessage("Subdomain is required")
            .MaximumLength(50).WithMessage("Subdomain cannot exceed 50 characters")
            .MinimumLength(3).WithMessage("Subdomain must be at least 3 characters")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Subdomain must contain only lowercase letters, numbers, and hyphens (e.g., 'acme-corp')");

        // Admin First Name
        RuleFor(x => x.AdminFirstName)
            .NotEmpty().WithMessage("Admin first name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        // Admin Last Name
        RuleFor(x => x.AdminLastName)
            .NotEmpty().WithMessage("Admin last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        // Admin Email
        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        // Admin Password
        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

        // Company Size
        RuleFor(x => x.CompanySize)
            .GreaterThan(0).WithMessage("Company size must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Company size seems unrealistic");

        // Expected Users
        RuleFor(x => x.ExpectedUsers)
            .GreaterThan(0).WithMessage("Expected users must be greater than 0")
            .LessThanOrEqualTo(100000).WithMessage("Expected users must be realistic");

        // Expected Projects
        RuleFor(x => x.ExpectedProjects)
            .GreaterThan(0).WithMessage("Expected projects must be greater than 0")
            .LessThanOrEqualTo(50000).WithMessage("Expected projects must be realistic");

        // Optional: Business Requirements
        RuleFor(x => x.BusinessRequirements)
            .MaximumLength(2000).WithMessage("Business requirements cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.BusinessRequirements));

        // Optional: Industry
        RuleFor(x => x.Industry)
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Industry));

        // Optional: Contact Phone
        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format (use E.164 format, e.g., +1234567890)")
            .When(x => !string.IsNullOrEmpty(x.ContactPhone));

        // Optional: Company Website
        RuleFor(x => x.CompanyWebsite)
            .Must(BeAValidUrl).WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.CompanyWebsite));
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}