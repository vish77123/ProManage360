using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Service
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Current user's ID (from JWT "sub" claim)
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// Current user's tenant ID (for multi-tenancy)
        /// </summary>
        Guid TenantId { get; }

        /// <summary>
        /// Current user's email
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// User's roles (e.g., "Admin", "Manager", "Member")
        /// </summary>
        IEnumerable<string> Roles { get; }
    }
}
