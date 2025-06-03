using System.ComponentModel.DataAnnotations;

namespace Faoem.Common.Inputs;

public class UserInput
{
    /// <summary>
    /// 用户名
    /// </summary>
    [RegularExpression("^[a-zA-Z][a-zA-Z0-9_]{3,31}$")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户全名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; } = null!;
}