namespace WSharp.HealthChecks;

/// <summary>
/// Health check configuration options
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Enable detailed health check responses
    /// </summary>
    public bool EnableDetailedResponses { get; set; } = true;

    /// <summary>
    /// Health check endpoint path
    /// </summary>
    public string HealthEndpoint { get; set; } = "/health";

    /// <summary>
    /// Ready check endpoint path
    /// </summary>
    public string ReadyEndpoint { get; set; } = "/health/ready";

    /// <summary>
    /// Live check endpoint path
    /// </summary>
    public string LiveEndpoint { get; set; } = "/health/live";

    /// <summary>
    /// Timeout for health checks in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable SQL Server health checks
    /// </summary>
    public bool EnableSqlServer { get; set; } = false;

    /// <summary>
    /// SQL Server connection string
    /// </summary>
    public string? SqlServerConnectionString { get; set; }

    /// <summary>
    /// Enable MongoDB health checks
    /// </summary>
    public bool EnableMongoDb { get; set; } = false;

    /// <summary>
    /// MongoDB connection string
    /// </summary>
    public string? MongoDbConnectionString { get; set; }

    /// <summary>
    /// Enable Redis health checks
    /// </summary>
    public bool EnableRedis { get; set; } = false;

    /// <summary>
    /// Redis connection string
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Enable RabbitMQ health checks
    /// </summary>
    public bool EnableRabbitMQ { get; set; } = false;

    /// <summary>
    /// RabbitMQ connection string
    /// </summary>
    public string? RabbitMQConnectionString { get; set; }

    /// <summary>
    /// Enable Kafka health checks
    /// </summary>
    public bool EnableKafka { get; set; } = false;

    /// <summary>
    /// Kafka bootstrap servers
    /// </summary>
    public string? KafkaBootstrapServers { get; set; }
}
