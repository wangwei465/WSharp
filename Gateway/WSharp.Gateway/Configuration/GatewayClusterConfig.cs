namespace WSharp.Gateway.Configuration;

/// <summary>
/// 网关集群（后端服务）配置
/// </summary>
public class GatewayClusterConfig
{
    /// <summary>
    /// 集群 ID（唯一标识符）
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// 集群目标（后端服务）
    /// </summary>
    public Dictionary<string, DestinationConfig> Destinations { get; set; } = new();

    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public string? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// 健康检查配置
    /// </summary>
    public HealthCheckConfig? HealthCheck { get; set; }

    /// <summary>
    /// HTTP 客户端配置
    /// </summary>
    public HttpClientConfig? HttpClient { get; set; }
}

/// <summary>
/// 目标（后端服务实例）配置
/// </summary>
public class DestinationConfig
{
    /// <summary>
    /// 目标地址（URL）
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 目标健康检查端点
    /// </summary>
    public string? Health { get; set; }

    /// <summary>
    /// 目标元数据
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// 健康检查配置
/// </summary>
public class HealthCheckConfig
{
    /// <summary>
    /// 启用主动健康检查
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 健康检查间隔
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 健康检查超时时间
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 健康检查路径
    /// </summary>
    public string Path { get; set; } = "/health";
}

/// <summary>
/// HTTP 客户端配置
/// </summary>
public class HttpClientConfig
{
    /// <summary>
    /// 请求超时时间
    /// </summary>
    public TimeSpan? RequestTimeout { get; set; }

    /// <summary>
    /// 最大自动重定向次数
    /// </summary>
    public int? MaxAutomaticRedirections { get; set; }

    /// <summary>
    /// 危险：接受任何 SSL 证书
    /// </summary>
    public bool DangerouslyAcceptAnyServerCertificate { get; set; } = false;
}
