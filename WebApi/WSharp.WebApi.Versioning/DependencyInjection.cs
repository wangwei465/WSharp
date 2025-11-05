using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace WSharp.WebApi.Versioning;

/// <summary>
/// API 版本控制依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp API 版本控制服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpApiVersioning(
        this IServiceCollection services,
        Action<ApiVersioningOptions>? configureOptions = null)
    {
        var options = new ApiVersioningOptions();
        configureOptions?.Invoke(options);

        // 配置 API 版本控制
        services.AddApiVersioning(config =>
        {
            // 设置默认版本
            config.DefaultApiVersion = options.DefaultApiVersion;

            // 当未指定版本时使用默认版本
            config.AssumeDefaultVersionWhenUnspecified = options.AssumeDefaultVersionWhenUnspecified;

            // 在响应头中报告支持的版本
            config.ReportApiVersions = options.ReportApiVersions;

            // 配置版本读取器
            var readers = new List<IApiVersionReader>();

            if (options.EnableUrlSegmentVersioning)
            {
                readers.Add(new UrlSegmentApiVersionReader());
            }

            if (options.EnableQueryStringVersioning)
            {
                readers.Add(new QueryStringApiVersionReader(options.QueryStringParameterName));
            }

            if (options.EnableHeaderVersioning)
            {
                readers.Add(new HeaderApiVersionReader(options.HeaderName));
            }

            if (options.EnableMediaTypeVersioning)
            {
                readers.Add(new MediaTypeApiVersionReader(options.MediaTypeParameterName));
            }

            // 如果没有启用任何读取器，默认使用 URL 段读取器
            if (readers.Count == 0)
            {
                readers.Add(new UrlSegmentApiVersionReader());
            }

            // 如果启用了多个读取器，使用组合读取器
            config.ApiVersionReader = readers.Count == 1
                ? readers[0]
                : ApiVersionReader.Combine(readers.ToArray());
        });

        // 如果启用了 API Explorer，添加版本化的 API Explorer
        if (options.EnableApiExplorer)
        {
            services.AddVersionedApiExplorer(explorerOptions =>
            {
                // 设置分组名称格式
                explorerOptions.GroupNameFormat = options.GroupNameFormat;

                // 在 URL 中替换版本占位符
                explorerOptions.SubstituteApiVersionInUrl = options.SubstituteApiVersionInUrl;
            });
        }

        return services;
    }

    /// <summary>
    /// 添加带有 Swagger 集成的 WSharp API 版本控制
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpApiVersioningWithSwagger(
        this IServiceCollection services,
        Action<ApiVersioningOptions>? configureOptions = null)
    {
        // 添加基础版本控制
        services.AddWSharpApiVersioning(configureOptions);

        return services;
    }

    /// <summary>
    /// 获取 API 版本描述信息提供者
    /// </summary>
    /// <param name="services">服务提供者</param>
    /// <returns>API 版本描述提供者</returns>
    public static IApiVersionDescriptionProvider GetApiVersionDescriptionProvider(
        this IServiceProvider services)
    {
        return services.GetRequiredService<IApiVersionDescriptionProvider>();
    }
}
