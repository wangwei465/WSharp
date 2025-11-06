using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Gateway.Middleware;
using WSharp.Gateway.Transforms;
using Yarp.ReverseProxy.Configuration;

namespace WSharp.Gateway;

/// <summary>
/// 网关依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 网关和 YARP 反向代理
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项委托</param>
    public static IServiceCollection AddWSharpGateway(
        this IServiceCollection services,
        Action<GatewayOptions>? configureOptions = null)
    {
        var options = new GatewayOptions();
        configureOptions?.Invoke(options);

        services.Configure(configureOptions ?? (_ => { }));

        // 添加 YARP 反向代理
        var proxyBuilder = services.AddReverseProxy();

        // 添加自定义转换器
        services.AddSingleton<CustomRequestTransform>();
        services.AddSingleton<CustomResponseTransform>();

        return services;
    }

    /// <summary>
    /// 使用自定义配置提供程序添加 WSharp 网关
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configProvider">代理配置提供程序</param>
    public static IServiceCollection AddWSharpGateway(
        this IServiceCollection services,
        IProxyConfigProvider configProvider)
    {
        services.AddSingleton(configProvider);

        services.AddReverseProxy();

        services.AddSingleton<CustomRequestTransform>();
        services.AddSingleton<CustomResponseTransform>();

        return services;
    }

    /// <summary>
    /// 使用 WSharp 网关中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    public static IApplicationBuilder UseWSharpGateway(this IApplicationBuilder app)
    {
        // 添加日志记录中间件
        app.UseMiddleware<GatewayLoggingMiddleware>();

        // 使用 YARP 反向代理
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapReverseProxy(proxyPipeline =>
            {
                proxyPipeline.UseLoadBalancing();
            });
        });

        return app;
    }

    /// <summary>
    /// 使用自定义配置的 WSharp 网关
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <param name="configurePipeline">配置管道委托</param>
    public static IApplicationBuilder UseWSharpGateway(
        this IApplicationBuilder app,
        Action<IApplicationBuilder> configurePipeline)
    {
        app.UseMiddleware<GatewayLoggingMiddleware>();

        app.UseRouting();

        configurePipeline(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapReverseProxy(proxyPipeline =>
            {
                proxyPipeline.UseLoadBalancing();
            });
        });

        return app;
    }
}
