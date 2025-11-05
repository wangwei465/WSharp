using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Infrastructure.Caching;

/// <summary>
/// 缓存依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加内存缓存
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMemoryCaching(
        this IServiceCollection services,
        Action<CacheOptions>? configureOptions = null)
    {
        // 注册内存缓存
        services.AddMemoryCache();

        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CacheOptions>(options => { });
        }

        // 注册缓存服务
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }

    /// <summary>
    /// 添加分布式缓存
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDistributedCaching(
        this IServiceCollection services,
        Action<CacheOptions>? configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CacheOptions>(options => { });
        }

        // 注册缓存服务（需要先注册 IDistributedCache 实现）
        services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }

    /// <summary>
    /// 添加 Redis 缓存
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">Redis 连接字符串</param>
    /// <param name="instanceName">实例名称（键前缀）</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        string connectionString,
        string? instanceName = null,
        Action<CacheOptions>? configureOptions = null)
    {
        // 注册 Redis 分布式缓存
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = instanceName;
        });

        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CacheOptions>(options => { });
        }

        // 注册缓存服务
        services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }

    /// <summary>
    /// 添加 Redis 缓存（使用配置选项）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureRedisOptions">配置 Redis 选项</param>
    /// <param name="configureCacheOptions">配置缓存选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureRedisOptions,
        Action<CacheOptions>? configureCacheOptions = null)
    {
        // 注册 Redis 分布式缓存
        services.AddStackExchangeRedisCache(configureRedisOptions);

        // 配置选项
        if (configureCacheOptions != null)
        {
            services.Configure(configureCacheOptions);
        }
        else
        {
            services.Configure<CacheOptions>(options => { });
        }

        // 注册缓存服务
        services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }

    /// <summary>
    /// 添加混合缓存（多级缓存：L1 内存 + L2 分布式）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">Redis 连接字符串</param>
    /// <param name="instanceName">实例名称（键前缀）</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddHybridCaching(
        this IServiceCollection services,
        string connectionString,
        string? instanceName = null,
        Action<HybridCacheOptions>? configureOptions = null)
    {
        // 注册内存缓存
        services.AddMemoryCache();

        // 注册 Redis 分布式缓存
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = instanceName;
        });

        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<HybridCacheOptions>(options => { });
        }

        // 注册混合缓存服务
        services.AddSingleton<ICacheService, HybridCacheService>();

        return services;
    }

    /// <summary>
    /// 添加混合缓存（使用配置选项）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureRedisOptions">配置 Redis 选项</param>
    /// <param name="configureCacheOptions">配置缓存选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddHybridCaching(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureRedisOptions,
        Action<HybridCacheOptions>? configureCacheOptions = null)
    {
        // 注册内存缓存
        services.AddMemoryCache();

        // 注册 Redis 分布式缓存
        services.AddStackExchangeRedisCache(configureRedisOptions);

        // 配置选项
        if (configureCacheOptions != null)
        {
            services.Configure(configureCacheOptions);
        }
        else
        {
            services.Configure<HybridCacheOptions>(options => { });
        }

        // 注册混合缓存服务
        services.AddSingleton<ICacheService, HybridCacheService>();

        return services;
    }
}
