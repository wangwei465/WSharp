namespace WSharp.Distributed.ServiceDiscovery.LoadBalancers;

/// <summary>
/// Round-robin load balancer
/// </summary>
public class RoundRobinLoadBalancer : ILoadBalancer
{
    private int _currentIndex = -1;
    private readonly object _lock = new();

    public ServiceRegistration? SelectService(IList<ServiceRegistration> services)
    {
        if (services == null || services.Count == 0)
            return null;

        lock (_lock)
        {
            _currentIndex = (_currentIndex + 1) % services.Count;
            return services[_currentIndex];
        }
    }
}
