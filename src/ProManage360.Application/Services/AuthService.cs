using Microsoft.AspNetCore.Identity;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Repository;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.DTOs;
using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Services
{
    //public class AuthService : IAuthService
    //{
    //    private readonly IUserRepository _userRepo;
    //    private readonly IRoleRepository _roleRepo;
    //    private readonly IJwtTokenService _tokenService;
    //    private readonly IUserRoleRepository _userRoleRepo;

    //    public AuthService(
    //        IUserRepository userRepo,
    //        IRoleRepository roleRepo,
    //        IUserRoleRepository userRoleRepo,
    //        IJwtTokenService tokenService)
    //    {
    //        _userRepo = userRepo;
    //        _roleRepo = roleRepo;
    //        _userRoleRepo = userRoleRepo;
    //        _tokenService = tokenService;
    //    }

    //    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    //    {
    //        // 1. Create user
    //        var user = new User
    //        {
    //            Email = dto.Email,
    //            FullName = dto.FullName,
    //            TenantId = dto.TenantId,
    //            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
    //        };

    //        await _userRepo.AddAsync(user);

    //        // 2. Determine role
    //        var isFirstUserInTenant = !await _userRepo.AnyUserInTenantAsync(dto.TenantId);
    //        var roleName = isFirstUserInTenant ? "TenantAdmin" : "User";

    //        // 3. Fetch role
    //        var role = await _roleRepo.GetByNameAsync(roleName, dto.TenantId)
    //                   ?? throw new Exception($"Role '{roleName}' not found");

    //        // 4. Assign role
    //        await _userRoleRepo.AddAsync(new UserRole
    //        {
    //            UserId = user.Id,
    //            RoleId = role.Id
    //        });

    //        // 5. Return JWT
    //        return new AuthResponseDto
    //        {
    //            Token = _tokenService.GenerateToken(user, new List<string> { roleName })
    //        };
    //    }

    //    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    //    {
    //        var user = await _userRepo.GetByEmailAndTenantAsync(dto.Email, dto.TenantId);
    //        if (user == null) return null;

    //        var result = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
    //        if (!result) return null;

    //        var roles = await _userRoleRepo.GetRolesForUserAsync(user.Id, dto.TenantId);

    //        return new AuthResponseDto
    //        {
    //            Token = _tokenService.GenerateToken(user, roles)
    //        };
    //    }
    //}

}

