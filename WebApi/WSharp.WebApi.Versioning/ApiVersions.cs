using Microsoft.AspNetCore.Mvc;

namespace WSharp.WebApi.Versioning;

/// <summary>
/// API 版本常量定义
/// </summary>
public static class ApiVersions
{
    /// <summary>
    /// 版本 1.0
    /// </summary>
    public static readonly ApiVersion V1_0 = new(1, 0);

    /// <summary>
    /// 版本 2.0
    /// </summary>
    public static readonly ApiVersion V2_0 = new(2, 0);

    /// <summary>
    /// 版本 3.0
    /// </summary>
    public static readonly ApiVersion V3_0 = new(3, 0);

    /// <summary>
    /// 获取所有定义的版本
    /// </summary>
    public static IEnumerable<ApiVersion> All => new[]
    {
        V1_0,
        V2_0,
        V3_0
    };
}
