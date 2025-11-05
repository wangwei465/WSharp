namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息队列选项
/// </summary>
public class MessageQueueOptions
{
    /// <summary>
    /// 提供者类型（RabbitMQ、Kafka、Azure Service Bus 等）
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用自动重试
    /// </summary>
    public bool EnableAutoRetry { get; set; } = true;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// 重试延迟（毫秒）
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;

    /// <summary>
    /// 是否启用死信队列
    /// </summary>
    public bool EnableDeadLetterQueue { get; set; } = true;

    /// <summary>
    /// 死信队列名称前缀
    /// </summary>
    public string DeadLetterQueuePrefix { get; set; } = "dlq-";

    /// <summary>
    /// 预取数量（消费者一次获取的消息数）
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>
    /// 消息过期时间（毫秒，0 表示不过期）
    /// </summary>
    public int MessageTtlMilliseconds { get; set; } = 0;

    /// <summary>
    /// 其他配置
    /// </summary>
    public Dictionary<string, string> AdditionalSettings { get; set; } = new();
}

/// <summary>
/// 队列配置
/// </summary>
public class QueueConfiguration
{
    /// <summary>
    /// 队列名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否持久化
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// 是否自动删除（当没有消费者时）
    /// </summary>
    public bool AutoDelete { get; set; } = false;

    /// <summary>
    /// 是否独占（只有一个消费者）
    /// </summary>
    public bool Exclusive { get; set; } = false;

    /// <summary>
    /// 队列参数
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; } = new();
}

/// <summary>
/// 主题配置
/// </summary>
public class TopicConfiguration
{
    /// <summary>
    /// 主题名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分区数量
    /// </summary>
    public int Partitions { get; set; } = 1;

    /// <summary>
    /// 副本因子
    /// </summary>
    public short ReplicationFactor { get; set; } = 1;

    /// <summary>
    /// 主题参数
    /// </summary>
    public Dictionary<string, string> Configurations { get; set; } = new();
}
