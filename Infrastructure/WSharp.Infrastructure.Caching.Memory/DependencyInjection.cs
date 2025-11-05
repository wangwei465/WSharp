using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// 内存缓存依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 内存缓存服务（基础版本）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpMemoryCaching(
        this IServiceCollection services,
        Action<MemoryCacheOptions>? configureOptions = null)
    {
        // 配置内存缓存选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<MemoryCacheOptions>(options => { });
        }

        // 获取临时配置以设置 MemoryCache
        var tempOptions = new MemoryCacheOptions();
        configureOptions?.Invoke(tempOptions);

        // 配置底层 MemoryCache
        services.AddMemoryCache(memoryCacheOptions =>
        {
            if (tempOptions.SizeLimit.HasValue)
            {
                memoryCacheOptions.SizeLimit = tempOptions.SizeLimit.Value;
            }

            if (tempOptions.CompactionPercentage > 0)
            {
                memoryCacheOptions.CompactionPercentage = tempOptions.CompactionPercentage;
            }

            if (tempOptions.ExpirationScanFrequencyMinutes > 0)
            {
                memoryCacheOptions.ExpirationScanFrequency = TimeSpan.FromMinutes(tempOptions.ExpirationScanFrequencyMinutes);
            }
        });

        // 注册内存缓存服务
        services.TryAddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }

    /// <summary>
    /// 添加带统计功能的 WSharp 内存缓存服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpMemoryCachingWithStats(
        this IServiceCollection services,
        Action<MemoryCacheOptions>? configureOptions = null)
    {
        // 添加基础内存缓存
        services.AddWSharpMemoryCaching(options =>
        {
            options.EnableStatistics = true;
            configureOptions?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 添加完整的 WSharp 内存缓存服务（包括所有功能）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpMemoryCachingFull(
        this IServiceCollection services,
        Action<MemoryCacheOptions>? configureOptions = null)
    {
        // 添加基础内存缓存，启用所有功能
        services.AddWSharpMemoryCaching(options =>
        {
            options.EnableStatistics = true;
            options.EnablePatternMatching = true;
            options.TrackSize = true;
            configureOptions?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 添加内存缓存服务（简化版，使用默认配置）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="keyPrefix">键前缀</param>
    /// <param name="defaultExpirationMinutes">默认过期时间（分钟）</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpMemoryCaching(
        this IServiceCollection services,
        string? keyPrefix = null,
        int defaultExpirationMinutes = 30)
    {
        return services.AddWSharpMemoryCaching(options =>
        {
            if (!string.IsNullOrEmpty(keyPrefix))
            {
                options.KeyPrefix = keyPrefix;
            }
            options.DefaultExpirationMinutes = defaultExpirationMinutes;
        });
    }

    /// <summary>
    /// 添加带容量限制的内存缓存服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="sizeLimit">大小限制（字节）</param>
    /// <param name="countLimit">数量限制</param>
    /// <param name="configureOptions">额外配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpMemoryCachingWithLimit(
        this IServiceCollection services,
        long? sizeLimit = null,
        int? countLimit = null,
        Action<MemoryCacheOptions>? configureOptions = null)
    {
        return services.AddWSharpMemoryCaching(options =>
        {
            options.SizeLimit = sizeLimit;
            options.CountLimit = countLimit;
            options.TrackSize = sizeLimit.HasValue;
            options.EnableStatistics = true;
            configureOptions?.Invoke(options);
        });
    }

    /// <summary>
    /// 添加分布式内存缓存（用于多实例场景，需要额外配置）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// 注意：内存缓存本质上是进程内的，无法跨实例共享。
    /// 如果需要分布式缓存，请使用 Redis 或其他分布式缓存方案。
    /// 此方法主要用于保持 API 一致性。
    /// </remarks>
    public static IServiceCollection AddWSharpDistributedMemoryCaching(
        this IServiceCollection services,
        Action<MemoryCacheOptions>? configureOptions = null)
    {
        // 添加分布式缓存接口（基于内存）
        services.AddDistributedMemoryCache();

        // 添加 WSharp 内存缓存
        return services.AddWSharpMemoryCaching(configureOptions);
    }
}
