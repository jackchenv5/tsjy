using Faoem.Common.DbContexts;
using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Common.Extensions;
using Faoem.Common.Services.User;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.UserRole;

internal class UserRoleService(
    CommonDbContext commonDbContext,
    IUserService userService
) : IUserRoleService
{
    public async Task<List<UserRoleDto>> GetUserRoleDtoAsync(long userId)
    {
        var user = await userService.GetUserAsync(userId);
        
        if (user is null)
        {
            throw new AppException($"The user with id [{userId}] is not found.", 404);
        }

        var hasRoles = commonDbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId);

        var userRoles = await commonDbContext.Roles
            .Select(r => r.ToUserRoleDto(hasRoles.Contains(r.Id)))
            .ToListAsync();

        return userRoles;
    }

    public async Task UpdateUserRolesAsync(long userId, List<long> roleIds)
    {
        var user = await userService.GetUserAsync(userId);
        
        if (user is null)
        {
            throw new AppException($"The user with id [{userId}] is not found.", 404);
        }

        // 确定每个角色都存在
        // 一般情况下，用户所属的角色数量应远小于所有角色数量，而 id 是主键
        // 因此，逐个查询的性能开销不大
        foreach (var roleId in roleIds)
        {
            var role = await commonDbContext.Roles.FindAsync(roleId);
            if (role is null)
            {
                throw new AppException("Invalid role ids.", 400);
            }
        }

        // 查找 UserRole 表中 userId 对应的记录，删除 roleId 不在 roleIds 中的记录
        var removeRoles = commonDbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Where(ur => !roleIds.Contains(ur.RoleId));

        commonDbContext.UserRoles.RemoveRange(removeRoles);

        foreach (var roleId in roleIds)
        {
            var userRole = await commonDbContext.UserRoles.FindAsync(user.Id, roleId);
            if (userRole is null)
            {
                await commonDbContext.UserRoles.AddAsync(new Models.UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                });
            }
        }

        await commonDbContext.SaveChangesAsync();
    }

    public async Task<List<Models.Role>> GetUserRolesAsync(long userId)
    {
        return await commonDbContext.UserRoles.Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<List<UserDto>> GetUsersAsync(long roleId)
    {
        var users = await commonDbContext.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.User.ToUserDto())
            .ToListAsync();

        return users;
    }
}