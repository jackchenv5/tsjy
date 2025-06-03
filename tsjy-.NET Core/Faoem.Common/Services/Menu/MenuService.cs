using Faoem.Common.DbContexts;
using Faoem.Common.Exceptions;
using Faoem.Common.Inputs;
using Faoem.Common.Services.Role;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.Menu;

internal class MenuService(CommonDbContext commonDbContext, IRoleService roleService) : IMenuService
{
    public async Task<List<Models.Menu>> GetMenuAsync()
    {
        var allMenus = await commonDbContext.Menus.OrderBy(menu => menu.Order).ToListAsync();
        var rootMenu = allMenus.Where(e => e.ParentId == null).ToList();

        return rootMenu;
    }

    public Task<List<Models.Menu>> GetMenusAsync()
    {
        return commonDbContext.Menus.OrderBy(menu => menu.Order).ToListAsync();
    }

    public async Task<Models.Menu> GetMenuAsync(long menuId)
    {
        var menu = await commonDbContext.Menus.FindAsync(menuId);
        if (menu is null)
        {
            throw new AppException("The menu is not found.", 404);
        }

        return menu;
    }

    public async Task<Models.Menu> AddMenuAsync(MenuInput menuInput)
    {
        var lowerLabel = menuInput.Label.ToLower();
        var lowerRoute = menuInput.Route?.ToLower();
        var exist = await commonDbContext.Menus.AnyAsync(menu =>
            menu.LowerLabel == lowerLabel && !string.IsNullOrEmpty(menu.LowerRoute) && menu.LowerRoute == lowerRoute);

        if (exist)
        {
            throw new AppException("The menu already exist.", 400);
        }

        var menu = new Models.Menu
        {
            Label = menuInput.Label,
            IsSubMenu = menuInput.IsSubMenu,
            ParentId = menuInput.ParentId,
            Route = menuInput.Route,
            Order = menuInput.Order
        };

        // 如果新添加的菜单没有指定顺序，则将其放在最后
        if (menuInput.Order == 0)
        {
            var lastOrder = await commonDbContext.Menus.OrderByDescending(m => m.Id)
                .Select(m => m.Order)
                .FirstOrDefaultAsync();
            menu.Order = lastOrder + 1;
        }

        await commonDbContext.AddAsync(menu);
        await commonDbContext.SaveChangesAsync();

        var role = await roleService.GetSysAdminRole();
        await commonDbContext.RoleMenus.AddAsync(new Models.RoleMenu
        {
            RoleId = role.Id,
            MenuId = menu.Id
        });
        await commonDbContext.SaveChangesAsync();

        return menu;
    }

    public async Task UpdateMenuAsync(long menuId, MenuInput menuInput)
    {
        var menu = await commonDbContext.Menus.FindAsync(menuId);
        if (menu is null)
        {
            throw new AppException("The menu does not exist.", 400);
        }

        var lowerLabel = menuInput.Label.ToLower();
        var lowerRoute = menuInput.Route?.ToLower();
        var exist = await commonDbContext.Menus.AnyAsync(m =>
            m.LowerLabel == lowerLabel && !string.IsNullOrEmpty(m.LowerRoute) && m.LowerRoute == lowerRoute);

        if (exist)
        {
            throw new AppException("The menu already exist.", 400);
        }

        menu.Label = menuInput.Label;
        menu.IsSubMenu = menuInput.IsSubMenu;
        menu.Order = menuInput.Order;
        menu.ParentId = menuInput.ParentId;
        menu.Route = menuInput.Route;

        await commonDbContext.SaveChangesAsync();
    }

    public async Task UpdateMenuAsync(List<Models.Menu> menus)
    {
        var validMenus = menus.Where(inputMenu => commonDbContext.Menus.Any(menu => inputMenu.Id == menu.Id));
        if (validMenus.Count() != menus.Count)
        {
            throw new AppException("Invalid menu list.", 400);
        }

        foreach (var menu in menus)
        {
            var model = await commonDbContext.Menus.FindAsync(menu.Id);
            if (model is null)
            {
                continue;
            }

            model.Order = menu.Order;
            model.ParentId = menu.ParentId;
        }

        await commonDbContext.SaveChangesAsync();
    }

    public async Task DeleteMenuAsync(long menuId)
    {
        var menu = await commonDbContext.Menus.FindAsync(menuId);

        if (menu is null)
        {
            throw new AppException("The menu does not exist.", 400);
        }

        commonDbContext.Remove(menu);
        await commonDbContext.SaveChangesAsync();
    }
}