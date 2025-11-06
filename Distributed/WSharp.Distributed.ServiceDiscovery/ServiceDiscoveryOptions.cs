namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// 服务发现配置选项
/// </summary>
public class ServiceDiscoveryOptions
{
    /// <summary>
    /// Consul 服务器地址
    /// </summary>
    public string ConsulAddress { get; set; } = "http://localhost:8500";

    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// 服务ID（唯一标识符）
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 服务主机地址
    /// </summary>
    public string ServiceAddress { get; set; } = string.Empty;

    /// <summary>
    /// 服务端口
    /// </summary>
    public int ServicePort { get; set; }

    /// <summary>
    /// 服务标签（用于元数据）
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 健康检查端点
    /// </summary>
    public string? HealthCheckEndpoint { get; set; }

    /// <summary>
    /// 健康检查间隔时间（秒）
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// 健康检查超时时间（秒）
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// 关键服务注销时间（例如："30s", "1m"）
    /// </summary>
    public string DeregisterCriticalServiceAfter { get; set; } = "30s";

    /// <summary>
    /// 启用启动时自动服务注册
    /// </summary>
    public bool EnableAutoRegistration { get; set; } = true;

    /// <summary>
    /// 启用关闭时自动服务注销
    /// </summary>
    public bool EnableAutoDeregistration { get; set; } = true;

    /// <summary>
    /// 服务元数据
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
