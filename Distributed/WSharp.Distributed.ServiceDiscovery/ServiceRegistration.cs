namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// 服务注册信息
/// </summary>
public class ServiceRegistration
{
    /// <summary>
    /// 服务ID
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// 服务地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 服务端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 服务标签
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 服务元数据
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// 健康状态
    /// </summary>
    public string HealthStatus { get; set; } = "Unknown";

    /// <summary>
    /// 完整的服务URL
    /// </summary>
    public string ServiceUrl => $"{Address}:{Port}";
}
