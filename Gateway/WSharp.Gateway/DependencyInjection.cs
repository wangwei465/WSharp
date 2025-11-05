using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Gateway.Middleware;
using WSharp.Gateway.Transforms;
using Yarp.ReverseProxy.Configuration;

namespace WSharp.Gateway;

/// <summary>
/// Dependency injection extensions for gateway
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add WSharp Gateway with YARP reverse proxy
    /// </summary>
    public static IServiceCollection AddWSharpGateway(
        this IServiceCollection services,
        Action<GatewayOptions>? configureOptions = null)
    {
        var options = new GatewayOptions();
        configureOptions?.Invoke(options);

        services.Configure(configureOptions ?? (_ => { }));

        // Add YARP reverse proxy
        var proxyBuilder = services.AddReverseProxy();

        // Add custom transforms
        services.AddSingleton<CustomRequestTransform>();
        services.AddSingleton<CustomResponseTransform>();

        return services;
    }

    /// <summary>
    /// Add WSharp Gateway with custom config provider
    /// </summary>
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
    /// Use WSharp Gateway middleware
    /// </summary>
    public static IApplicationBuilder UseWSharpGateway(this IApplicationBuilder app)
    {
        // Add logging middleware
        app.UseMiddleware<GatewayLoggingMiddleware>();

        // Use YARP reverse proxy
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
    /// Use WSharp Gateway with custom configuration
    /// </summary>
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
