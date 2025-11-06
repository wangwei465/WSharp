namespace WSharp.Gateway.Configuration;

/// <summary>
/// 网关路由配置
/// </summary>
public class GatewayRouteConfig
{
    /// <summary>
    /// 路由 ID（唯一标识符）
    /// </summary>
    public string RouteId { get; set; } = string.Empty;

    /// <summary>
    /// 要路由到的集群 ID
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// 路由匹配模式
    /// </summary>
    public string Match { get; set; } = string.Empty;

    /// <summary>
    /// 路由顺序（值越小优先级越高）
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// 此路由是否需要身份认证
    /// </summary>
    public bool RequireAuthentication { get; set; } = false;

    /// <summary>
    /// 此路由所需的角色
    /// </summary>
    public string[]? RequiredRoles { get; set; }

    /// <summary>
    /// 为此路由启用速率限制
    /// </summary>
    public bool EnableRateLimiting { get; set; } = false;

    /// <summary>
    /// 此路由每分钟的速率限制
    /// </summary>
    public int? RateLimitPerMinute { get; set; }

    /// <summary>
    /// 请求超时时间（秒）
    /// </summary>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// 要添加到请求的自定义头部
    /// </summary>
    public Dictionary<string, string>? RequestHeaders { get; set; }

    /// <summary>
    /// 要从请求中移除的头部
    /// </summary>
    public string[]? RemoveRequestHeaders { get; set; }

    /// <summary>
    /// 要添加到响应的自定义头部
    /// </summary>
    public Dictionary<string, string>? ResponseHeaders { get; set; }

    /// <summary>
    /// 要从响应中移除的头部
    /// </summary>
    public string[]? RemoveResponseHeaders { get; set; }
}
