using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } // Admin, Manager, Member
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Multi-tenancy
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }


}
