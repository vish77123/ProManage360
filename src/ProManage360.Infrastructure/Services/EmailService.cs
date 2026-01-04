namespace ProManage360.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(
        string toEmail,
        string firstName,
        string subdomain,
        string tier,
        DateTime trialEndsAt)
    {
        // TODO: Implement actual email sending using SendGrid, AWS SES, or SMTP
        _logger.LogInformation(
            "Sending welcome email to {Email} for tenant {Subdomain} ({Tier}). Trial ends: {TrialEnds}",
            toEmail, subdomain, tier, trialEndsAt);

        await Task.CompletedTask;

        // Simulated email content
        var emailBody = $@"
            Hi {firstName},

            Welcome to ProManage 360!

            Your account has been created successfully.
            
            Subdomain: {subdomain}.promanage360.com
            Subscription: {tier}
            Trial Period: {(trialEndsAt - DateTime.UtcNow).Days} days remaining

            Get started: https://{subdomain}.promanage360.com/dashboard

            Best regards,
            ProManage 360 Team
        ";

        _logger.LogDebug("Email content: {EmailBody}", emailBody);
    }

    public async Task SendEmailVerificationAsync(
        string toEmail,
        string firstName,
        string verificationToken)
    {
        _logger.LogInformation("Sending email verification to {Email}", toEmail);

        await Task.CompletedTask;

        var emailBody = $@"
            Hi {firstName},

            Please verify your email by clicking the link below:

            https://promanage360.com/verify-email?token={verificationToken}

            This link will expire in 24 hours.

            Best regards,
            ProManage 360 Team
        ";

        _logger.LogDebug("Email verification content: {EmailBody}", emailBody);
    }
}