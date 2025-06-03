using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Common.Services.RolePermission;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionController(IRolePermissionService rolePermissionService) : ControllerBase
{
    [HttpGet("{roleId}")]
    [Description("获取指定角色的访问权限")]
    public async Task<ActionResult<List<RolePermissionDto>>> GetRolePermissionsAsync(long roleId)
    {
        return await rolePermissionService.GetRolePermissionAsync(roleId);
    }

    [HttpPut("{roleId}")]
    [Description("更新指定角色的访问权限")]
    public async Task<IActionResult> UpdateRolePermissionsAsync(long roleId, List<Guid> permissionIds)
    {
        await rolePermissionService.UpdateRolePermissionAsync(roleId, permissionIds);

        return NoContent();
    }
}