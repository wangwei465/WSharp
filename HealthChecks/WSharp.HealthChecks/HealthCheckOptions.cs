namespace WSharp.HealthChecks;

/// <summary>
/// 健康检查配置选项
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// 启用详细的健康检查响应
    /// </summary>
    public bool EnableDetailedResponses { get; set; } = true;

    /// <summary>
    /// 健康检查端点路径
    /// </summary>
    public string HealthEndpoint { get; set; } = "/health";

    /// <summary>
    /// 就绪检查端点路径
    /// </summary>
    public string ReadyEndpoint { get; set; } = "/health/ready";

    /// <summary>
    /// 存活检查端点路径
    /// </summary>
    public string LiveEndpoint { get; set; } = "/health/live";

    /// <summary>
    /// 健康检查超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 启用 SQL Server 健康检查
    /// </summary>
    public bool EnableSqlServer { get; set; } = false;

    /// <summary>
    /// SQL Server 连接字符串
    /// </summary>
    public string? SqlServerConnectionString { get; set; }

    /// <summary>
    /// 启用 MongoDB 健康检查
    /// </summary>
    public bool EnableMongoDb { get; set; } = false;

    /// <summary>
    /// MongoDB 连接字符串
    /// </summary>
    public string? MongoDbConnectionString { get; set; }

    /// <summary>
    /// 启用 Redis 健康检查
    /// </summary>
    public bool EnableRedis { get; set; } = false;

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// 启用 RabbitMQ 健康检查
    /// </summary>
    public bool EnableRabbitMQ { get; set; } = false;

    /// <summary>
    /// RabbitMQ 连接字符串
    /// </summary>
    public string? RabbitMQConnectionString { get; set; }

    /// <summary>
    /// 启用 Kafka 健康检查
    /// </summary>
    public bool EnableKafka { get; set; } = false;

    /// <summary>
    /// Kafka 引导服务器地址
    /// </summary>
    public string? KafkaBootstrapServers { get; set; }
}
