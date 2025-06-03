using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Common.Inputs;
using Faoem.Common.Models;
using Faoem.Common.Services.Role;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    [Description("获取角色列表")]
    public async Task<ActionResult<PagedDto<Role>>> GetAsync(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20
    )
    {
        return await roleService.GetRoleAsync(pageIndex, pageSize);
    }


    [HttpGet("{id}")]
    [Description("获取指定角色")]
    public async Task<ActionResult<Role?>> GetAsync(long id)
    {
        return await roleService.GetRoleAsync(id);
    }

    [HttpPost]
    [Description("添加角色")]
    public async Task<ActionResult<Role>> AddRoleAsync(RoleInput roleInput)
    {
        var role = await roleService.AddRoleAsync(roleInput);

        return CreatedAtAction("Get", new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    [Description("更新角色")]
    public async Task<IActionResult> UpdateRoleAsync(long id, RoleInput roleInput)
    {
        await roleService.UpdateRoleAsync(id, roleInput);
        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    [Description("删除角色")]
    public async Task<IActionResult> DeleteRoleAsync(long id)
    {
        await roleService.DeleteRoleAsync(id);
        return new NoContentResult();
    }
}