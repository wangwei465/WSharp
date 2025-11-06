using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WSharp.Distributed.ServiceDiscovery.LoadBalancers;

namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// 服务发现的依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加基于 Consul 的服务发现
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscovery(
        this IServiceCollection services,
        Action<ServiceDiscoveryOptions> configureOptions)
    {
        // 配置选项
        services.Configure(configureOptions);

        // 获取用于 Consul 客户端配置的选项
        var options = new ServiceDiscoveryOptions();
        configureOptions(options);

        // 注册 Consul 客户端
        services.AddSingleton<IConsulClient>(provider =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(options.ConsulAddress);
            }));

        // 注册服务发现
        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();

        // 注册负载均衡器
        services.AddSingleton<ILoadBalancer, RandomLoadBalancer>();

        // 注册后台服务以实现自动注册/注销
        services.AddHostedService<ServiceDiscoveryHostedService>();

        return services;
    }

    /// <summary>
    /// 添加基于 Consul 的服务发现和自定义负载均衡器
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscovery<TLoadBalancer>(
        this IServiceCollection services,
        Action<ServiceDiscoveryOptions> configureOptions)
        where TLoadBalancer : class, ILoadBalancer
    {
        services.AddWSharpServiceDiscovery(configureOptions);

        // 替换负载均衡器
        services.AddSingleton<ILoadBalancer, TLoadBalancer>();

        return services;
    }

    /// <summary>
    /// 添加无自动注册的服务发现（用于仅客户端场景）
    /// </summary>
    public static IServiceCollection AddWSharpServiceDiscoveryClient(
        this IServiceCollection services,
        string consulAddress = "http://localhost:8500")
    {
        // 注册 Consul 客户端
        services.AddSingleton<IConsulClient>(provider =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(consulAddress);
            }));

        // 注册服务发现但不进行自动注册
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
