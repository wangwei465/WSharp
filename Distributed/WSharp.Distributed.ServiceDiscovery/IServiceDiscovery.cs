namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// 服务发现接口
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// 注册服务
    /// </summary>
    Task<bool> RegisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 注销服务
    /// </summary>
    Task<bool> DeregisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称获取所有健康的服务实例
    /// </summary>
    Task<IEnumerable<ServiceRegistration>> GetHealthyServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称获取所有服务实例（包括不健康的）
    /// </summary>
    Task<IEnumerable<ServiceRegistration>> GetAllServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用负载均衡获取单个服务实例
    /// </summary>
    Task<ServiceRegistration?> GetServiceAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有已注册的服务名称
    /// </summary>
    Task<IEnumerable<string>> GetAllServiceNamesAsync(CancellationToken cancellationToken = default);
}
