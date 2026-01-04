using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Repository
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task<List<string>> GetRolesForUserAsync(Guid userId, Guid tenantId);
    }
}
