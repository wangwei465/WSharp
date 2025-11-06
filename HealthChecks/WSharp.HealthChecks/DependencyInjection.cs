using HealthChecks.Kafka;
using HealthChecks.MongoDb;
using HealthChecks.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WSharp.HealthChecks;

/// <summary>
/// 健康检查的依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 健康检查服务
    /// </summary>
    public static IServiceCollection AddWSharpHealthChecks(
        this IServiceCollection services,
        Action<HealthCheckOptions>? configureOptions = null)
    {
        var options = new HealthCheckOptions();
        configureOptions?.Invoke(options);

        services.Configure(configureOptions ?? (_ => { }));

        var healthChecksBuilder = services.AddHealthChecks();

        // 添加内存健康检查
        healthChecksBuilder.AddCheck<MemoryHealthCheck>(
            "memory",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "memory", "system" });

        // 添加磁盘空间健康检查
        healthChecksBuilder.AddCheck<DiskSpaceHealthCheck>(
            "disk_space",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "disk", "system" });

        // SQL Server 健康检查
        if (options.EnableSqlServer && !string.IsNullOrEmpty(options.SqlServerConnectionString))
        {
            healthChecksBuilder.AddSqlServer(
                options.SqlServerConnectionString,
                name: "sqlserver",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "sqlserver" },
                timeout: TimeSpan.FromSeconds(options.TimeoutSeconds));
        }

        // MongoDB 健康检查
        if (options.EnableMongoDb && !string.IsNullOrEmpty(options.MongoDbConnectionString))
        {
            healthChecksBuilder.AddMongoDb(
                options.MongoDbConnectionString,
                name: "mongodb",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "mongodb" },
                timeout: TimeSpan.FromSeconds(options.TimeoutSeconds));
        }

        // Redis 健康检查
        if (options.EnableRedis && !string.IsNullOrEmpty(options.RedisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                options.RedisConnectionString,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "cache", "redis" },
                timeout: TimeSpan.FromSeconds(options.TimeoutSeconds));
        }

        // RabbitMQ 健康检查
        if (options.EnableRabbitMQ && !string.IsNullOrEmpty(options.RabbitMQConnectionString))
        {
            healthChecksBuilder.AddRabbitMQ(
                rabbitConnectionString: options.RabbitMQConnectionString,
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "messaging", "rabbitmq" },
                timeout: TimeSpan.FromSeconds(options.TimeoutSeconds));
        }

        // Kafka 健康检查
        if (options.EnableKafka && !string.IsNullOrEmpty(options.KafkaBootstrapServers))
        {
            healthChecksBuilder.AddKafka(
                setup =>
                {
                    setup.BootstrapServers = options.KafkaBootstrapServers;
                },
                name: "kafka",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "messaging", "kafka" },
                timeout: TimeSpan.FromSeconds(options.TimeoutSeconds));
        }

        return services;
    }

    /// <summary>
    /// 添加自定义健康检查
    /// </summary>
    public static IHealthChecksBuilder AddCustomHealthCheck<THealthCheck>(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
        where THealthCheck : class, IHealthCheck
    {
        return builder.AddCheck<THealthCheck>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags,
            timeout);
    }
}
