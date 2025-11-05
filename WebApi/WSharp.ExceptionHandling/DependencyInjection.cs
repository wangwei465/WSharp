using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WSharp.ExceptionHandling.Filters;
using WSharp.ExceptionHandling.Middleware;

namespace WSharp.ExceptionHandling;

/// <summary>
/// 异常处理依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 异常处理服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpExceptionHandling(
        this IServiceCollection services,
        Action<ExceptionHandlingOptions>? configureOptions = null)
    {
        var options = new ExceptionHandlingOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);

        // 注册异常过滤器（供用户在 MVC 配置中使用）
        if (!options.UseMiddleware)
        {
            services.AddScoped<GlobalExceptionFilter>();
        }

        return services;
    }

    /// <summary>
    /// 使用 WSharp 异常处理中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseWSharpExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
