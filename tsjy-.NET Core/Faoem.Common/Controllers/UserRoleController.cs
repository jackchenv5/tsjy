using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Common.Services.UserRole;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController(IUserRoleService userRoleService) : ControllerBase
{
    [HttpGet("{userId}")]
    [Description("获取用户拥有的角色")]
    public async Task<ActionResult<List<UserRoleDto>>> GetUserRolesAsync(long userId)
    {
        return await userRoleService.GetUserRoleDtoAsync(userId);
    }

    [HttpPut("{userId}")]
    [Description("更新用户拥有的角色")]
    public async Task<IActionResult> UpdateUserRolesAsync(long userId, List<long> roleIds)
    {
        await userRoleService.UpdateUserRolesAsync(userId, roleIds);

        return NoContent();
    }
}