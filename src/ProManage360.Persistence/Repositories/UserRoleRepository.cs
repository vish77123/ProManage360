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
    //public class UserRoleRepository : IUserRoleRepository
    //{
    //    private readonly ApplicationDbContext _context;

    //    public UserRoleRepository(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task AddAsync(UserRole userRole)
    //    {
    //        _context.UserRoles.Add(userRole);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task<List<string>> GetRolesForUserAsync(Guid userId, Guid tenantId)
    //    {
    //        return await _context.UserRoles
    //            .Where(ur => ur.UserId == userId)
    //            .Select(ur => ur.Role.Name)
    //            .ToListAsync();
    //    }
    //}
}
