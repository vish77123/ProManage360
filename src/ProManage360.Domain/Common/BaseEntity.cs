using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Domain.Common
{
    /// <summary>
    /// Base entity class - all entities inherit from this
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
    }

    /// <summary>
    /// Auditable entity - includes creation and modification tracking
    /// </summary>
    public abstract class AuditableEntity : BaseEntity, IAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Tenant entity - multi-tenant with audit tracking
    /// </summary>
    public abstract class TenantEntity : AuditableEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
    }

    /// <summary>
    /// Soft-deletable tenant entity - supports soft delete with multi-tenancy
    /// </summary>
    public abstract class SoftDeletableTenantEntity : TenantEntity, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
