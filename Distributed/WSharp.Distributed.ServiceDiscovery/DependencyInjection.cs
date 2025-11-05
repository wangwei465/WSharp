using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WSharp.Distributed.ServiceDiscovery.LoadBalancers;

namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Dependency injection extensions for service discovery
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add service discovery with Consul
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscovery(
        this IServiceCollection services,
        Action<ServiceDiscoveryOptions> configureOptions)
    {
        // Configure options
        services.Configure(configureOptions);

        // Get options for Consul client configuration
        var options = new ServiceDiscoveryOptions();
        configureOptions(options);

        // Register Consul client
        services.AddSingleton<IConsulClient>(provider =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(options.ConsulAddress);
            }));

        // Register service discovery
        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();

        // Register load balancers
        services.AddSingleton<ILoadBalancer, RandomLoadBalancer>();

        // Register hosted service for automatic registration/deregistration
        services.AddHostedService<ServiceDiscoveryHostedService>();

        return services;
    }

    /// <summary>
    /// Add service discovery with Consul and custom load balancer
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscovery<TLoadBalancer>(
        this IServiceCollection services,
        Action<ServiceDiscoveryOptions> configureOptions)
        where TLoadBalancer : class, ILoadBalancer
    {
        services.AddWSharpServiceDiscovery(configureOptions);

        // Replace load balancer
        services.AddSingleton<ILoadBalancer, TLoadBalancer>();

        return services;
    }

    /// <summary>
    /// Add service discovery without automatic registration (for client-only scenarios)
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscoveryClient(
        this IServiceCollection services,
        string consulAddress = "http://localhost:8500")
    {
        // Register Consul client
        services.AddSingleton<IConsulClient>(provider =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(consulAddress);
            }));

        // Register service discovery without auto-registration
        services.Configure<ServiceDiscoveryOptions>(options =>
        {
            options.ConsulAddress = consulAddress;
            options.EnableAutoRegistration = false;
            options.EnableAutoDeregistration = false;
        });

        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
        services.AddSingleton<ILoadBalancer, RandomLoadBalancer>();

        return services;
    }
}
