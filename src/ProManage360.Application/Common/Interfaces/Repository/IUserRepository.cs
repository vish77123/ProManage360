using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId);
        Task<User> AddAsync(User user);
        Task<List<Role>> GetRolesForUserAsync(Guid userId, Guid tenantId);
        Task<bool> AnyUserInTenantAsync(Guid tenantId);
    }

}
