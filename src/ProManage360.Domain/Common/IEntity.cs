using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Domain.Common
{
    /// <summary>
    /// Base marker interface for all entities
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// Interface for tenant-scoped entities (multi-tenancy)
    /// </summary>
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
    }

    /// <summary>
    /// Interface for soft-deletable entities
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }

    /// <summary>
    /// Interface for auditable entities (tracking creation and modification)
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        Guid? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
