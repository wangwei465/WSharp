namespace WSharp.Distributed.ServiceDiscovery.LoadBalancers;

/// <summary>
/// 用于服务选择的负载均衡器接口
/// </summary>
public interface ILoadBalancer
{
    /// <summary>
    /// 从可用实例中选择一个服务实例
    /// </summary>
    ServiceRegistration? SelectService(IList<ServiceRegistration> services);
}
