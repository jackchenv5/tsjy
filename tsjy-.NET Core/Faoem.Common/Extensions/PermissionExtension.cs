using Faoem.Common.DbContexts;
using Faoem.Common.Models;
using Faoem.Common.Services.Permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Common.Extensions;

public static class PermissionExtension
{
    public static WebApplication MapPermission(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var permissionService = sp.GetRequiredService<IPermissionService>();
        permissionService.RefreshPermissionAsync().Wait();

        return app;
    }

    public static WebApplication UpdateSysAdminPermission(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var commonDbContext = sp.GetRequiredService<CommonDbContext>();

        var sysadminRole = commonDbContext.Roles.FirstOrDefault(role => role.LowerRoleName == "sysadmin");
        var permissions = commonDbContext.Permissions.ToList();

        if (sysadminRole is null || permissions.Count == 0)
        {
            return app;
        }

        // 如果系统管理员不具有权限，添加权限
        foreach (var permission in
                 from permission in permissions
                 let rolePermission = commonDbContext.RolePermissions
                     .FirstOrDefault(rolePermission =>
                         rolePermission.RoleId == sysadminRole.Id &&
                         rolePermission.PermissionId == permission.Id)
                 where rolePermission is null
                 select permission)
        {
            commonDbContext.RolePermissions.Add(new RolePermission
            {
                RoleId = sysadminRole.Id,
                PermissionId = permission.Id
            });
        }

        commonDbContext.SaveChanges();

        return app;
    }
}