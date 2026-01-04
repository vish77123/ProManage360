using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Repository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName, Guid tenantId);
        Task<Role> AddAsync(Role role);
    }
}
