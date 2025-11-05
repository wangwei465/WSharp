namespace WSharp.Distributed.ServiceDiscovery.LoadBalancers;

/// <summary>
/// Load balancer interface for service selection
/// </summary>
public interface ILoadBalancer
{
    /// <summary>
    /// Select a service instance from available instances
    /// </summary>
    ServiceRegistration? SelectService(IList<ServiceRegistration> services);
}
