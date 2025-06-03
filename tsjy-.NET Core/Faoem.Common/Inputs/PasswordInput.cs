namespace Faoem.Common.Inputs;

public class PasswordInput
{
    // 前端传入的密码需要经过 SHA256 哈希计算 

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = null!;

    /// <summary>
    /// 确认新密码
    /// </summary>
    public string ConfirmNewPassword { get; set; } = null!;
}