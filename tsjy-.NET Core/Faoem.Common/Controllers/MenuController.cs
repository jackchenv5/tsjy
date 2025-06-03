using System.ComponentModel;
using Faoem.Common.Inputs;
using Faoem.Common.Models;
using Faoem.Common.Services.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController(IMenuService menuService) : ControllerBase
{
    [HttpGet]
    [Description("获取结构化菜单")]
    public async Task<ActionResult<List<Menu>>> GetAsync()
    {
        return await menuService.GetMenuAsync();
    }

    [HttpGet("{id}")]
    [Description("获取指定菜单")]
    public async Task<ActionResult<Menu>> GetAsync(long id)
    {
        return await menuService.GetMenuAsync(id);
    }

    [HttpPost]
    [Description("添加菜单")]
    public async Task<ActionResult<Menu>> AddAsync(MenuInput menuInput)
    {
        var menu = await menuService.AddMenuAsync(menuInput);
        return CreatedAtAction("Get", new { id = menu.Id }, menu);
    }

    [HttpPut("{id}")]
    [Description("更新菜单")]
    public async Task<IActionResult> UpdateAsync(long id, MenuInput menuInput)
    {
        await menuService.UpdateMenuAsync(id, menuInput);
        return NoContent();
    }

    [HttpPut]
    [Description("更新菜单顺序")]
    public async Task<IActionResult> UpdateAsync(List<Menu> menus)
    {
        await menuService.UpdateMenuAsync(menus);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Description("删除指定菜单")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await menuService.DeleteMenuAsync(id);
        return NoContent();
    }
}