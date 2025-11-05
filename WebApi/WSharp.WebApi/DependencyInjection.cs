using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WSharp.WebApi.Filters;
using WSharp.WebApi.Middleware;

namespace WSharp.WebApi;

/// <summary>
/// WebApi 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp WebApi 服务
    /// </summary>
    public static IServiceCollection AddWSharpWebApi(this IServiceCollection services)
    {
        // 添加控制器服务
        services.AddControllers(options =>
        {
            // 添加全局过滤器
            options.Filters.Add<ModelValidationFilter>();
            options.Filters.Add<AsyncGlobalExceptionFilter>();
            options.Filters.Add<PerformanceMonitorFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // 禁用默认的模型验证响应
            options.SuppressModelStateInvalidFilter = true;
        });

        // 配置 JSON 序列化选项
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // 使用驼峰命名
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                // 忽略 null 值
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                // 允许数字作为字符串
                options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            });

        return services;
    }

    /// <summary>
    /// 添加 WSharp WebApi 服务（带响应包装）
    /// </summary>
    public static IServiceCollection AddWSharpWebApiWithWrapper(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ModelValidationFilter>();
            options.Filters.Add<ResponseWrapperFilter>();
            options.Filters.Add<AsyncGlobalExceptionFilter>();
            options.Filters.Add<PerformanceMonitorFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            });

        return services;
    }

    /// <summary>
    /// 使用 WSharp WebApi 中间件
    /// </summary>
    public static IApplicationBuilder UseWSharpWebApi(this IApplicationBuilder app)
    {
        // 添加请求 ID 中间件
        app.UseMiddleware<RequestIdMiddleware>();

        // 添加请求日志中间件
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }

    /// <summary>
    /// 添加 CORS 配置
    /// </summary>
    public static IServiceCollection AddWSharpCors(
        this IServiceCollection services,
        Action<CorsOptions> configureCors)
    {
        var corsOptions = new CorsOptions();
        configureCors(corsOptions);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                if (corsOptions.AllowedOrigins.Length > 0)
                {
                    builder.WithOrigins(corsOptions.AllowedOrigins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (corsOptions.AllowedMethods.Length > 0)
                {
                    builder.WithMethods(corsOptions.AllowedMethods);
                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (corsOptions.AllowedHeaders.Length > 0 && corsOptions.AllowedHeaders[0] == "*")
                {
                    builder.AllowAnyHeader();
                }
                else if (corsOptions.AllowedHeaders.Length > 0)
                {
                    builder.WithHeaders(corsOptions.AllowedHeaders);
                }

                if (corsOptions.AllowCredentials)
                {
                    builder.AllowCredentials();
                }

                builder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsOptions.MaxAge));
            });
        });

        return services;
    }
}
