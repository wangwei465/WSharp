using Microsoft.AspNetCore.Mvc;

namespace WSharp.WebApi.Versioning;

/// <summary>
/// 版本化 API 控制器基类
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class VersionedApiControllerBase : ControllerBase
{
    /// <summary>
    /// 获取当前请求的 API 版本
    /// </summary>
    protected ApiVersion CurrentApiVersion => HttpContext.GetRequestedApiVersion() ?? new ApiVersion(1, 0);

    /// <summary>
    /// 获取当前 API 版本的字符串表示
    /// </summary>
    protected string CurrentApiVersionString => CurrentApiVersion.ToString();

    /// <summary>
    /// 检查是否为指定版本
    /// </summary>
    /// <param name="version">要检查的版本</param>
    /// <returns>是否为指定版本</returns>
    protected bool IsApiVersion(ApiVersion version)
    {
        return CurrentApiVersion == version;
    }

    /// <summary>
    /// 检查是否为指定的主版本号
    /// </summary>
    /// <param name="majorVersion">主版本号</param>
    /// <returns>是否为指定的主版本号</returns>
    protected bool IsApiMajorVersion(int majorVersion)
    {
        return CurrentApiVersion.MajorVersion == majorVersion;
    }
}

/// <summary>
/// 版本化 API 控制器基类（无路由模板）
/// </summary>
/// <remarks>
/// 当需要完全自定义路由时使用此基类
/// </remarks>
[ApiController]
public abstract class VersionedApiControllerBaseWithoutRoute : ControllerBase
{
    /// <summary>
    /// 获取当前请求的 API 版本
    /// </summary>
    protected ApiVersion CurrentApiVersion => HttpContext.GetRequestedApiVersion() ?? new ApiVersion(1, 0);

    /// <summary>
    /// 获取当前 API 版本的字符串表示
    /// </summary>
    protected string CurrentApiVersionString => CurrentApiVersion.ToString();

    /// <summary>
    /// 检查是否为指定版本
    /// </summary>
    /// <param name="version">要检查的版本</param>
    /// <returns>是否为指定版本</returns>
    protected bool IsApiVersion(ApiVersion version)
    {
        return CurrentApiVersion == version;
    }

    /// <summary>
    /// 检查是否为指定的主版本号
    /// </summary>
    /// <param name="majorVersion">主版本号</param>
    /// <returns>是否为指定的主版本号</returns>
    protected bool IsApiMajorVersion(int majorVersion)
    {
        return CurrentApiVersion.MajorVersion == majorVersion;
    }
}
