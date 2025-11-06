namespace WSharp.Gateway;

/// <summary>
/// 网关配置选项
/// </summary>
public class GatewayOptions
{
    /// <summary>
    /// 启用请求/响应日志记录
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// 启用分布式追踪
    /// </summary>
    public bool EnableTracing { get; set; } = true;

    /// <summary>
    /// 启用速率限制
    /// </summary>
    public bool EnableRateLimiting { get; set; } = false;

    /// <summary>
    /// 每分钟速率限制请求数
    /// </summary>
    public int RateLimitPerMinute { get; set; } = 1000;

    /// <summary>
    /// 启用身份认证
    /// </summary>
    public bool EnableAuthentication { get; set; } = false;

    /// <summary>
    /// 用于身份认证的 JWT 密钥
    /// </summary>
    public string? JwtSecretKey { get; set; }

    /// <summary>
    /// 启用负载均衡
    /// </summary>
    public bool EnableLoadBalancing { get; set; } = true;

    /// <summary>
    /// 负载均衡策略（FirstHealthy、Random、RoundRobin、LeastRequests、PowerOfTwoChoices）
    /// </summary>
    public string LoadBalancingPolicy { get; set; } = "RoundRobin";

    /// <summary>
    /// 启用健康检查
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// 健康检查间隔（秒）
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// 健康检查超时时间（秒）
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// 启用响应缓存
    /// </summary>
    public bool EnableResponseCaching { get; set; } = false;

    /// <summary>
    /// 响应缓存持续时间（秒）
    /// </summary>
    public int ResponseCacheDurationSeconds { get; set; } = 60;
}
