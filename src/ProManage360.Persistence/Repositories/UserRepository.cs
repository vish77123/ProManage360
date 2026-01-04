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
    //public class UserRepository : IUserRepository
    //{
    //    private readonly ApplicationDbContext _db;

    //    public UserRepository(ApplicationDbContext db)
    //    {
    //        _db = db;
    //    }

    //    public async Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId)
    //    {
    //        return await _db.Users
    //            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
    //    }

    //    public async Task<User> AddAsync(User user)
    //    {
    //        _db.Users.Add(user);
    //        await _db.SaveChangesAsync();
    //        return user;
    //    }

    //    public async Task<List<Role>> GetRolesForUserAsync(Guid userId, Guid tenantId)
    //    {
    //        return await _db.UserRoles
    //            .Where(ur => ur.UserId == userId)
    //            .Select(ur => ur.Role)
    //            .ToListAsync();
    //    }

    //    public async Task<bool> AnyUserInTenantAsync(Guid tenantId)
    //    {
    //        return await _db.Users.AnyAsync(u => u.TenantId == tenantId);
    //    }
    //}
}
