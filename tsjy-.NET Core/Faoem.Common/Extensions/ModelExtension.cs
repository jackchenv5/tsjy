using Faoem.Common.Dtos;
using Faoem.Common.Models;

namespace Faoem.Common.Extensions;

public static class ModelExtension
{
    internal static UserDto ToUserDto(this User userModel)
    {
        return new UserDto
        {
            Id = userModel.Id,
            Username = userModel.Username,
            LowerUsername = userModel.LowerUsername,
            FullName = userModel.FullName,
            CreatedAt = userModel.CreatedAt,
            LastLogin = userModel.LastLogin,
            Email = userModel.Email,
            LowerEmail = userModel.LowerEmail
        };
    }

    internal static UserRoleDto ToUserRoleDto(this Role role, bool hasRole)
    {
        return new UserRoleDto
        {
            RoleId = role.Id,
            RoleName = role.RoleName,
            HasRole = hasRole
        };
    }

    internal static RolePermissionDto ToRolePermissionDto(this Permission permission, bool hasPermission)
    {
        return new RolePermissionDto
        {
            PermissionId = permission.Id,
            Route = permission.Route,
            HttpMethod = permission.HttpMethod,
            ControllerName = permission.ControllerName,
            ActionDescription = permission.ActionDescription,
            HasPermission = hasPermission
        };
    }

    internal static RoleMenuDto ToRoleMenuDto(this Menu menu, List<long> hasMenus, bool hasMenu)
    {
        return new RoleMenuDto
        {
            Id = menu.Id,
            Label = menu.Label,
            IsSubMenu = menu.IsSubMenu,
            Order = menu.Order,
            ParentId = menu.ParentId,
            Route = menu.Route,
            Children = menu.Children?.Select(m =>
                m.ToRoleMenuDto(hasMenus, hasMenus.Contains(m.Id))).ToList(),
            HasMenu = hasMenu
        };
    }
}