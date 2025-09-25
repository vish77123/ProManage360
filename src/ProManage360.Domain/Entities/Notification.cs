using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public required string Message { get; set; }
        public required string Type { get; set; } // Info, Warning, Alert
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // User target
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Multi-tenancy
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }

}
