namespace WSharp.Distributed.ServiceDiscovery.LoadBalancers;

/// <summary>
/// Random load balancer
/// </summary>
public class RandomLoadBalancer : ILoadBalancer
{
    private readonly Random _random = new();

    public ServiceRegistration? SelectService(IList<ServiceRegistration> services)
    {
        if (services == null || services.Count == 0)
            return null;

        var index = _random.Next(services.Count);
        return services[index];
    }
}
