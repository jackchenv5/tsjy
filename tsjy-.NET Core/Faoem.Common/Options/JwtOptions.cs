namespace Faoem.Common.Options;

public class JwtOptions
{
    /// <summary>
    /// 签发方密钥
    /// </summary>
    public string IssuerSigningKey { get; set; } = "db122ce2f63a4c40823e43901761a664";

    /// <summary>
    /// 签发方
    /// </summary>
    public string ValidIssuer { get; set; } = "faoem";

    /// <summary>
    /// 签收方
    /// </summary>
    public string ValidAudience { get; set; } = "faoem";

    /// <summary>
    /// 过期时间容错值，单位：秒
    /// </summary>
    public int ClockSkew { get; set; } = 5;

    /// <summary>
    /// 过期时间，单位：分钟
    /// </summary>
    public int TokenLifeTime { get; set; } = 15;

    /// <summary>
    /// 刷新 token 过期时间，单位：分钟
    /// </summary>
    public int? RefreshTokenLifeTime { get; set; }
}