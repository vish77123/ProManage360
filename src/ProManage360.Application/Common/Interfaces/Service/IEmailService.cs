using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Service
{
    public interface IEmailService
    {
        /// <summary>
        /// Send welcome email to new tenant
        /// </summary>
        Task SendWelcomeEmailAsync(
            string toEmail,
            string firstName,
            string subdomain,
            string tier,
            DateTime trialEndsAt);

        /// <summary>
        /// Send email verification link
        /// </summary>
        Task SendEmailVerificationAsync(
            string toEmail,
            string firstName,
            string verificationToken);
    }
}
