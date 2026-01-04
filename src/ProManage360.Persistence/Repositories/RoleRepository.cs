using Microsoft.EntityFrameworkCore;
using ProManage360.Application.Common.Interfaces.Repository;
using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Persistence.Repositories
{
    //public class RoleRepository : IRoleRepository
    //{
    //    private readonly ApplicationDbContext _db;

    //    public RoleRepository(ApplicationDbContext db)
    //    {
    //        _db = db;
    //    }

    //    public async Task<Role?> GetByNameAsync(string roleName, Guid tenantId)
    //    {
    //        return await _db.Roles
    //                        .FirstOrDefaultAsync(r => r.Name == roleName && r.TenantId == tenantId);
    //    }

    //    public async Task<Role> AddAsync(Role role)
    //    {
    //        _db.Roles.Add(role);
    //        await _db.SaveChangesAsync();
    //        return role;
    //    }
    //}

}
