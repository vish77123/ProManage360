using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Domain.Enums;

namespace ProManage360.Application.Features.Auth.Command.RegisterTenant
{
    public class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
    {
        private readonly IApplicationDbContext _context;
        public RegisterTenantCommandValidator(IApplicationDbContext context) 
        {
            _context = context;

            RuleFor(x => x.TenantName)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-\.&']+$")
                .WithMessage("Company name contains invalid characters");

            RuleFor(x => x.Subdomain)
                .NotEmpty().WithMessage("Subdomain is required")
                .MinimumLength(3).WithMessage("Subdomain must be at least 3 characters")
                .MaximumLength(50).WithMessage("Subdomain cannot exceed 50 characters")
                .Matches(@"^[a-z0-9][a-z0-9-]*[a-z0-9]$")
                    .WithMessage("Subdomain must start/end with letter or number, contain only lowercase letters, numbers, and hyphens")
                .MustAsync(BeUniqueSubdomain)
                    .WithMessage("This subdomain is already taken");

            // First Name Validation
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            // Last Name Validation
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            // Email Validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters")
                .MustAsync(BeUniqueEmail)
                    .WithMessage("This email is already registered");
            
            // Password Validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

            // Subscription Tier Validation
            RuleFor(x => x.Tier)
                .IsInEnum().WithMessage("Invalid subscription tier")
                .Must(tier => tier == SubscriptionTier.Free || tier == SubscriptionTier.Professional)
                    .WithMessage("Only Free and Professional tiers are available for self-service registration");

            // Phone Number Validation (Optional)
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                    .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // Website Validation (Optional)
            RuleFor(x => x.CompanyWebsite)
                .Must(BeValidUrl)
                    .WithMessage("Invalid website URL")
                .When(x => !string.IsNullOrEmpty(x.CompanyWebsite));

        }

        /// <summary>
        /// Check if subdomain is unique (async database check)
        /// </summary>
        private async Task<bool> BeUniqueSubdomain(string subdomain, CancellationToken cancellationToken)
        {
            var exists = await _context.Tenants
                .AnyAsync(t => t.Subdomain == subdomain.ToLower(), cancellationToken);

            return !exists; // Return true if NOT exists (validation passes)
        }

        /// <summary>
        /// Check if email is unique across all tenants
        /// </summary>
        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var exists = await _context.Users
                .Where(u => !u.IsDeleted) // Exclude soft-deleted users
                .AnyAsync(u => u.Email == email.ToLower(), cancellationToken);

            return !exists; // Return true if NOT exists
        }

        /// <summary>
        /// Validate URL format
        /// </summary>
        private bool BeValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true; // Null/empty is valid (optional field)

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
