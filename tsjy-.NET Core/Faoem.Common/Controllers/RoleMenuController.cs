using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Common.Models;
using Faoem.Common.Services.RoleMenu;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleMenuController(IRoleMenuService roleMenuService) : ControllerBase
{
    [HttpGet]
    [Description("获取当前用户的菜单")]
    public async Task<ActionResult<List<Menu>>> GetMenuAsync()
    {
        return await roleMenuService.GetUserMenuAsync();
    }

    [HttpGet("{roleId}")]
    [Description("获取指定角色的菜单")]
    public async Task<ActionResult<List<RoleMenuDto>>> GetMenuAsync(long roleId)
    {
        return await roleMenuService.GetRoleMenuAsync(roleId);
    }

    [HttpPut("{roleId}")]
    [Description("更新指定角色的菜单")]
    public async Task<IActionResult> UpdateMenuAsync(long roleId, List<long> menuIds)
    {
        await roleMenuService.UpdateRoleMenuAsync(roleId, menuIds);
        return NoContent();
    }
}