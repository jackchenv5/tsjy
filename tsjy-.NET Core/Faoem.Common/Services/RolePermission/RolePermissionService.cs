using Faoem.Common.DbContexts;
using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Common.Extensions;
using Faoem.Common.Services.Role;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.RolePermission;

internal class RolePermissionService(IRoleService roleService, CommonDbContext commonDbContext) : IRolePermissionService
{
    public async Task<List<RolePermissionDto>> GetRolePermissionAsync(long roleId)
    {
        var role = await roleService.GetRoleAsync(roleId);

        if (role is null)
        {
            throw new AppException($"The role with id [{roleId}] is not found.", 404);
        }

        var hasPermissions = commonDbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == role.Id)
            .Select(rolePermission => rolePermission.PermissionId);

        var rolePermissions = await commonDbContext.Permissions
            .OrderBy(permission => permission.ControllerName)
            .Select(permission => permission.ToRolePermissionDto(hasPermissions.Contains(permission.Id)))
            .ToListAsync();

        return rolePermissions;
    }

    public async Task UpdateRolePermissionAsync(long roleId, List<Guid> permissionIds)
    {
        var role = await roleService.GetRoleAsync(roleId);
        
        if (role is null)
        {
            throw new AppException($"The role with id [{roleId}] is not found.", 404);
        }

        // 确定每个权限都存在
        foreach (var permissionId in permissionIds)
        {
            var exist = await commonDbContext.Permissions.AnyAsync(permission => permission.Id == permissionId);
            if (!exist)
            {
                throw new AppException("Invalid permission ids.", 400);
            }
        }

        // 查找 RolePermission 表中 roleId 对应的记录，删除 permissionId 不在 permissionIds 中的记录
        var removePermissions = commonDbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == role.Id)
            .Where(rolePermission => !permissionIds.Contains(rolePermission.PermissionId));

        commonDbContext.RolePermissions.RemoveRange(removePermissions);

        foreach (var permissionId in permissionIds)
        {
            var exist = await commonDbContext.RolePermissions
                .AnyAsync(rolePermission =>
                    rolePermission.RoleId == role.Id && rolePermission.PermissionId == permissionId);
            if (!exist)
            {
                await commonDbContext.RolePermissions.AddAsync(new Models.RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }
        }

        await commonDbContext.SaveChangesAsync();
    }
}