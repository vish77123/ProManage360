using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Domain.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public required string Action { get; set; }
        public required string EntityName { get; set; } // Project, TaskItem, etc.
        public Guid EntityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Performed By
        public Guid PerformedByUserId { get; set; }
        public User PerformedByUser { get; set; } = null!;

        // Multi-tenancy
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }

}
