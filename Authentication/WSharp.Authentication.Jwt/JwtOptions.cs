namespace WSharp.Authentication.Jwt;

/// <summary>
/// JWT 配置选项
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 发行者
    /// </summary>
    public string Issuer { get; set; } = "WSharp";

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = "WSharp";

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// 刷新令牌过期时间（天）
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// 是否验证发行者
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// 是否验证受众
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// 是否验证生命周期
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// 是否验证签名密钥
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;

    /// <summary>
    /// 时钟偏移（秒）
    /// </summary>
    public int ClockSkewSeconds { get; set; }
}
