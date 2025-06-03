using System.ComponentModel;
using Faoem.Common.Models;
using Faoem.Common.Services.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingController(ISettingService settingService) : ControllerBase
{
    [HttpGet("AppName")]
    [AllowAnonymous]
    public async Task<ActionResult<Setting>> GetAppNameAsync()
    {
        return await settingService.GetSettingAsync("AppName");
    }

    [HttpPut("AppName")]
    [Description("更新应用名称")]
    public async Task<IActionResult> UpdateAppNameAsync([FromBody] string? appName = default)
    {
        await settingService.UpdateSettingAsync("AppName", appName);

        return Ok();
    }

    [HttpGet("{key}")]
    [Description("获取指定设置")]
    public async Task<ActionResult<Setting>> GetSettingAsync(string key)
    {
        return await settingService.GetSettingAsync(key);
    }

    [HttpPut("{key}")]
    [Description("更新指定设置")]
    public async Task<IActionResult> UpdateSettingAsync(string key, [FromBody] string? value = default)
    {
        await settingService.UpdateSettingAsync(key, value);

        return Ok();
    }

    [HttpGet("HomePage")]
    [AllowAnonymous]
    public async Task<ActionResult<Setting>> GetHomePageAsync()
    {
        var setting = await settingService.GetSettingAsync("HomePage");
        setting.Value ??= "/";

        return setting;
    }

    [HttpPut("HomePage")]
    [Description("更新自定义首页")]
    public async Task<IActionResult> UpdateHomePageAsync([FromBody] string? homePage = default)
    {
        await settingService.UpdateSettingAsync("HomePage", homePage);

        return Ok();
    }
}