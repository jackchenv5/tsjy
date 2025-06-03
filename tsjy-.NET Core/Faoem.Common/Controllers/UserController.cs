using System.ComponentModel;
using Faoem.Common.Dtos;
using Faoem.Common.Inputs;
using Faoem.Common.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("IsAuthenticated")]
    [AllowAnonymous]
    public async Task<JsonResult> IsAuthenticated()
    {
        var user = await userService.GetCurrentUserAsync();
        return new JsonResult(new { IsAuthenticated = user is not null });
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> LoginAsync(LoginInput loginInput)
    {
        return await userService.LoginAsync(loginInput);
    }

    [HttpPost("CaptchaLogin")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> LoginAsync(CaptchaInput captchaInput)
    {
        return await userService.CaptchaLoginAsync(captchaInput);
    }

    [HttpGet]
    [Description("获取用户列表")]
    public async Task<ActionResult<PagedDto<UserDto>>> GetAsync(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20
    )
    {
        return await userService.GetUserAsync(pageIndex, pageSize);
    }

    [HttpGet("{id}")]
    [Description("获取指定用户")]
    public async Task<ActionResult<UserDto?>> GetAsync(long id)
    {
        return await userService.GetUserAsync(id);
    }

    [HttpPost]
    [Description("添加用户")]
    public async Task<ActionResult<UserDto>> AddUserAsync(UserInput userInput)
    {
        var userDto = await userService.AddUserAsync(userInput);

        return CreatedAtAction("Get", new { id = userDto.Id }, userDto);
    }

    [HttpPut("{id}")]
    [Description("更新指定用户")]
    public async Task<IActionResult> UpdateUserAsync(long id, UserInput userInput)
    {
        await userService.UpdateUserAsync(id, userInput);
        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    [Description("删除指定用户")]
    public async Task<IActionResult> DeleteUserAsync(long id)
    {
        await userService.DeleteUserAsync(id);
        return new NoContentResult();
    }

    [HttpPost("GetCaptcha")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCaptchaAsync(EmailInput emailInput)
    {
        await userService.GetCaptchaAsync(emailInput);

        return Ok();
    }

    [HttpPut("Info/{id}")]
    [Description("更新用户信息")]
    public async Task<IActionResult> UpdateUserInfoAsync(long id, UserInfoInput userInfoInput)
    {
        await userService.UpdateUserInfoAsync(id, userInfoInput);
        return NoContent();
    }

    [HttpPut("Password/{id}")]
    [Description("修改密码")]
    public async Task<IActionResult> UpdatePasswordAsync(long id, PasswordInput passwordInput)
    {
        await userService.UpdatePasswordAsync(id, passwordInput);
        return NoContent();
    }


#if DEBUG
    [HttpGet("AllowAnonymous")]
    [AllowAnonymous]
    public async Task<string> AllowAnonymous()
    {
        return await Task.FromResult("allow anonymous");
    }

    [HttpGet("AuthenticationRequired")]
    public string AuthenticationRequired()
    {
        return "authentication required";
    }
#endif
}