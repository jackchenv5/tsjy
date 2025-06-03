using Faoem.Common.DbContexts;
using Faoem.Common.Inputs;
using Faoem.Common.Models;
using Faoem.Common.Services.Menu;
using Faoem.Common.Services.Setting;
using Faoem.Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Common.Extensions;

internal static class SeedDataExtension
{
    internal static CommonDbContext AddSeedData(this CommonDbContext commonDbContext)
    {
        commonDbContext.AddUserSeedData();
        commonDbContext.AddRoleSeedData();
        commonDbContext.AddUserRoleSeedData();

        return commonDbContext;
    }

    private static void AddUserSeedData(this CommonDbContext commonDbContext)
    {
        if (commonDbContext.Users.Any())
        {
            return;
        }

        var sysadmin = new User
        {
            Username = "SysAdmin",
            FullName = "System Admin",
            Salt = Guid.NewGuid().ToString("N"),
        };

        // 前端传输的是密码哈希值
        var inputPassword = UserUtils.GetHashString("faoemapp");
        // 原始密码哈希值加盐后重新计算保存到数据库的密码哈希值
        sysadmin.PasswordHash = UserUtils.GetPasswordHash(inputPassword, sysadmin.Salt);
        sysadmin.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        commonDbContext.Users.Add(sysadmin);

#if DEBUG
        var testUser = new User
        {
            Username = "TestUser",
            FullName = "Test User",
            Salt = Guid.NewGuid().ToString("N"),
        };
        testUser.PasswordHash = UserUtils.GetPasswordHash(UserUtils.GetHashString("123123"), testUser.Salt);
        testUser.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        commonDbContext.Users.Add(testUser);
#endif


        commonDbContext.SaveChanges();
    }


    private static void AddRoleSeedData(this CommonDbContext commonDbContext)
    {
        if (commonDbContext.Roles.Any())
        {
            return;
        }

        var sysadminRole = new Role
        {
            RoleName = "SysAdmin"
        };

        commonDbContext.Roles.Add(sysadminRole);

#if DEBUG
        var testRole = new Role
        {
            RoleName = "TestRole"
        };

        commonDbContext.Roles.Add(testRole);
#endif

        commonDbContext.SaveChanges();
    }

    private static void AddUserRoleSeedData(this CommonDbContext commonDbContext)
    {
        if (commonDbContext.UserRoles.Any())
        {
            return;
        }

        var sysadminUser = commonDbContext.Users.FirstOrDefault(u => u.LowerUsername == "sysadmin");

        var sysadminRole = commonDbContext.Roles.FirstOrDefault(r => r.LowerRoleName == "sysadmin");

        if (sysadminUser is null)
        {
            throw new Exception("SysAdmin user is required.");
        }

        if (sysadminRole is null)
        {
            throw new Exception("SysAdmin role is required.");
        }

        var userRole = new UserRole
        {
            UserId = sysadminUser.Id,
            RoleId = sysadminRole.Id
        };

        commonDbContext.UserRoles.Add(userRole);
        commonDbContext.SaveChanges();
    }

    internal static WebApplication AddDefaultMenu(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var menuService = sp.GetRequiredService<IMenuService>();

        var menus = menuService.GetMenuAsync().Result;

        if (menus.Any(m => m.Route == "/settings"))
        {
            return app;
        }

        var order = menus.Count != 0 ? menus.OrderBy(m => m.Order).Last().Order : 0;

        var settingsMenu = menuService.AddMenuAsync(new MenuInput
        {
            IsSubMenu = true,
            Label = "Settings",
            Order = ++order,
            Route = "/settings"
        }).Result;

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "App Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/app"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "Menu Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/menu"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "Role Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/role"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "User Setting",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/user"
        }).Wait();

        menuService.AddMenuAsync(new MenuInput
        {
            Label = "User Profile",
            Order = ++order,
            ParentId = settingsMenu.Id,
            Route = "/settings/user-profile"
        }).Wait();

        return app;
    }

    internal static WebApplication AddDefaultSettings(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var settingService = sp.GetRequiredService<ISettingService>();

        var appNameSetting = settingService.GetSettingAsync("AppName").Result;

        if (string.IsNullOrEmpty(appNameSetting.Value))
        {
            settingService.UpdateSettingAsync("AppName", "FA OEM App").Wait();
        }

        var homePageSetting = settingService.GetSettingAsync("HomePage").Result;

        if (string.IsNullOrEmpty(homePageSetting.Value))
        {
            settingService.UpdateSettingAsync("HomePage", "/settings/menu").Wait();
        }

        return app;
    }
}