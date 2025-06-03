using Faoem.Common.DbContexts;
using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Common.Extensions;
using Faoem.Common.Services.Menu;
using Faoem.Common.Services.Role;
using Faoem.Common.Services.User;
using Faoem.Common.Services.UserRole;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.RoleMenu;

internal class RoleMenuService(
    CommonDbContext commonDbContext,
    IUserService userService,
    IRoleService roleService,
    IMenuService menuService,
    IUserRoleService userRoleService
) : IRoleMenuService
{
    public async Task<List<Models.Menu>> GetUserMenuAsync()
    {
        var user = await userService.GetCurrentUserAsync();

        if (user is null)
        {
            return [];
        }

        var roleIds = (await userRoleService.GetUserRolesAsync(user.Id))
            .Select(role => role.Id).ToList();

        var menus = await commonDbContext.RoleMenus
            .Where(roleMenu => roleIds.Contains(roleMenu.RoleId))
            .Select(roleMenu => roleMenu.Menu)
            .Distinct()
            .OrderBy(menu => menu.Order)
            .ToListAsync();

        return menus.Where(e => e.ParentId is null).ToList();
    }

    public async Task<List<RoleMenuDto>> GetRoleMenuAsync(long roleId)
    {
        var role = await roleService.GetRoleAsync(roleId);

        if (role is null)
        {
            throw new AppException($"The role with id [{roleId}] is not found.", 404);
        }
        
        var hasMenus = await commonDbContext.RoleMenus
            .Where(roleMenu => roleMenu.RoleId == role.Id)
            .Select(roleMenu => roleMenu.MenuId)
            .ToListAsync();

        var menus = await menuService.GetMenuAsync();

        return menus.Select(menu => menu.ToRoleMenuDto(hasMenus, hasMenus.Contains(menu.Id)))
            .ToList();
    }

    public async Task UpdateRoleMenuAsync(long roleId, List<long> menuIds)
    {
        var role = await commonDbContext.Roles.FindAsync(roleId);
        if (role is null)
        {
            throw new AppException("Invalid role id.", 400);
        }

        foreach (var menuId in menuIds)
        {
            var exist = await commonDbContext.Menus.AnyAsync(menu => menu.Id == menuId);
            if (!exist)
            {
                throw new AppException("Invalid menu ids.", 400);
            }
        }

        // 查找 RoleMenu 表中 roleId 对应的记录，删除 menuId 不在 menuIds 中的记录
        var removeMenus = commonDbContext.RoleMenus
            .Where(roleMenu => roleMenu.RoleId == role.Id)
            .Where(roleMenu => !menuIds.Contains(roleMenu.MenuId));

        commonDbContext.RoleMenus.RemoveRange(removeMenus);

        foreach (var menuId in menuIds)
        {
            var exist = await commonDbContext.RoleMenus
                .AnyAsync(roleMenu => roleMenu.RoleId == role.Id && roleMenu.MenuId == menuId);
            if (!exist)
            {
                await commonDbContext.RoleMenus.AddAsync(new Models.RoleMenu
                {
                    RoleId = role.Id,
                    MenuId = menuId
                });
            }
        }

        await commonDbContext.SaveChangesAsync();
    }
}