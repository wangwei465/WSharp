using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 缓存依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Redis 缓存服务（基础版本）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">Redis 连接字符串</param>
    /// <param name="instanceName">实例名称</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCaching(
        this IServiceCollection services,
        string connectionString,
        string? instanceName = null,
        Action<RedisCacheOptions>? configureOptions = null)
    {
        // 配置 Redis 选项
        services.Configure<RedisCacheOptions>(options =>
        {
            options.ConnectionString = connectionString;
            options.InstanceName = instanceName;
            configureOptions?.Invoke(options);
        });

        // 注册 Redis 连接
        services.TryAddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RedisCacheOptions>>().Value;
            var configurationOptions = BuildConfigurationOptions(options);
            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        // 注册 Redis 缓存服务
        services.TryAddSingleton<ICacheService, RedisCacheService>();

        // 如果启用了脚本功能，注册脚本服务
        var tempOptions = new RedisCacheOptions();
        configureOptions?.Invoke(tempOptions);

        if (tempOptions.EnableScripting)
        {
            services.TryAddSingleton<RedisScriptService>();
        }

        return services;
    }

    /// <summary>
    /// 添加 Redis 缓存服务（使用配置回调）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCaching(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new RedisCacheOptions();
        configureOptions(options);

        return services.AddWSharpRedisCaching(
            options.ConnectionString,
            options.InstanceName,
            configureOptions);
    }

    /// <summary>
    /// 添加带发布/订阅功能的 Redis 缓存服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">Redis 连接字符串</param>
    /// <param name="instanceName">实例名称</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCachingWithPubSub(
        this IServiceCollection services,
        string connectionString,
        string? instanceName = null,
        Action<RedisCacheOptions>? configureOptions = null)
    {
        // 添加基础 Redis 缓存服务
        services.AddWSharpRedisCaching(connectionString, instanceName, opt =>
        {
            opt.EnablePubSub = true;
            configureOptions?.Invoke(opt);
        });

        // 注册发布/订阅服务
        services.TryAddSingleton<IRedisPubSubService, RedisPubSubService>();

        return services;
    }

    /// <summary>
    /// 添加带发布/订阅功能的 Redis 缓存服务（使用配置回调）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCachingWithPubSub(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new RedisCacheOptions();
        configureOptions(options);

        return services.AddWSharpRedisCachingWithPubSub(
            options.ConnectionString,
            options.InstanceName,
            configureOptions);
    }

    /// <summary>
    /// 添加完整的 Redis 缓存服务（包括所有高级功能）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">Redis 连接字符串</param>
    /// <param name="instanceName">实例名称</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCachingFull(
        this IServiceCollection services,
        string connectionString,
        string? instanceName = null,
        Action<RedisCacheOptions>? configureOptions = null)
    {
        // 添加带发布/订阅的 Redis 缓存服务
        services.AddWSharpRedisCachingWithPubSub(connectionString, instanceName, opt =>
        {
            opt.EnableBatchOperations = true;
            opt.EnablePubSub = true;
            opt.EnableScripting = true;
            configureOptions?.Invoke(opt);
        });

        // 确保所有服务都已注册
        services.TryAddSingleton<RedisScriptService>();
        services.TryAddSingleton<IRedisPubSubService, RedisPubSubService>();

        return services;
    }

    /// <summary>
    /// 添加完整的 Redis 缓存服务（使用配置回调）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpRedisCachingFull(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new RedisCacheOptions();
        configureOptions(options);

        return services.AddWSharpRedisCachingFull(
            options.ConnectionString,
            options.InstanceName,
            configureOptions);
    }

    #region 辅助方法

    /// <summary>
    /// 构建 Redis 配置选项
    /// </summary>
    private static ConfigurationOptions BuildConfigurationOptions(RedisCacheOptions options)
    {
        var configOptions = ConfigurationOptions.Parse(options.ConnectionString);

        configOptions.ConnectTimeout = options.ConnectTimeout;
        configOptions.SyncTimeout = options.SyncTimeout;
        configOptions.AsyncTimeout = options.AsyncTimeout;
        configOptions.AllowAdmin = options.AllowAdmin;
        configOptions.Ssl = options.UseSsl;
        configOptions.AbortOnConnectFail = false; // 连接失败时不中断
        configOptions.ConnectRetry = options.RetryCount;

        if (!string.IsNullOrEmpty(options.Password))
        {
            configOptions.Password = options.Password;
        }

        return configOptions;
    }

    #endregion
}
