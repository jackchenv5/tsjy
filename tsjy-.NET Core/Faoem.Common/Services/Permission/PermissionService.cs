using System.ComponentModel;
using System.Security.Claims;
using Faoem.Common.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Common.Services.Permission;

internal class PermissionService(
    IHttpContextAccessor httpContextAccessor,
    CommonDbContext commonDbContext,
    IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider
) : IPermissionService
{
    public async Task<bool> CheckPermissionAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;

        var userIdStr = httpContext?.User.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userIdStr))
        {
            return false;
        }

        if (!long.TryParse(userIdStr, out var userId))
        {
            return false;
        }

        var endpoint = httpContext?.Features.Get<IEndpointFeature>()?
            .Endpoint as RouteEndpoint;
        var route = endpoint?.RoutePattern.RawText;
        var httpMethod = httpContext?.Request.Method;

        if (string.IsNullOrEmpty(route))
        {
            return false;
        }

        var roles = commonDbContext.UserRoles
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.RoleId);

        if (!await roles.AnyAsync())
        {
            return false;
        }

        var permissions = commonDbContext.RolePermissions
            .Where(rp => roles.Contains(rp.RoleId))
            .Include(rp => rp.Permission)
            .Where(rp => rp.Permission.HttpMethod == httpMethod)
            .Select(rp => rp.Permission.Route);

        return await permissions.AnyAsync(r => r == route);
    }

    public Task RefreshPermissionAsync()
    {
        // 获取动作列表
        var apiDescriptions = apiDescriptionGroupCollectionProvider
            .ApiDescriptionGroups.Items
            .SelectMany(g => g.Items)
            .ToList();

        // 从数据库中删除不存在的权限
        foreach (var permission in commonDbContext.Permissions)
        {
            if (apiDescriptions.Any(d =>
                    d.RelativePath == permission.Route &&
                    d.HttpMethod == permission.HttpMethod))
            {
                continue;
            }

            commonDbContext.Permissions.Remove(permission);
        }

        // 向数据库中添加新权限
        foreach (var apiDescription in apiDescriptions)
        {
            var route = apiDescription.RelativePath;
            var httpMethod = apiDescription.HttpMethod;

            if (route == null || httpMethod == null)
            {
                continue;
            }

            if (apiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            {
                continue;
            }

            // 该动作方法允许匿名访问，不作为权限处理
            var allowAnonymous =
                apiDescription.ActionDescriptor.EndpointMetadata.Where(m => m is AllowAnonymousAttribute);
            if (allowAnonymous.Any())
            {
                continue;
            }

            var controllerName = descriptor.ControllerName;
            var descriptionAttr = descriptor.MethodInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() as DescriptionAttribute;

            // 如果数据库中已经存在该动作方法对应的权限，仅更新描述
            if (commonDbContext.Permissions.Any(p =>
                    p.Route == apiDescription.RelativePath && p.HttpMethod == apiDescription.HttpMethod))
            {
                var permission = commonDbContext.Permissions
                    .First(p =>
                        p.Route == apiDescription.RelativePath && p.HttpMethod == apiDescription.HttpMethod);
                permission.ActionDescription = descriptionAttr?.Description;
                continue;
            }

            // 添加新权限
            commonDbContext.Permissions.Add(new Models.Permission
            {
                Route = route,
                HttpMethod = httpMethod,
                ControllerName = controllerName,
                ActionDescription = descriptionAttr?.Description
            });
        }

        commonDbContext.SaveChanges();

        return Task.CompletedTask;
    }
}