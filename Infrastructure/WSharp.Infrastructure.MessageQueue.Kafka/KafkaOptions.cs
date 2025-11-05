namespace WSharp.Infrastructure.MessageQueue.Kafka;

/// <summary>
/// Kafka 选项
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// Bootstrap 服务器地址
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// 消费者组 ID
    /// </summary>
    public string GroupId { get; set; } = "wsharp-consumer-group";

    /// <summary>
    /// 自动提交偏移量
    /// </summary>
    public bool EnableAutoCommit { get; set; } = false;

    /// <summary>
    /// 自动提交间隔（毫秒）
    /// </summary>
    public int AutoCommitIntervalMs { get; set; } = 5000;

    /// <summary>
    /// 会话超时（毫秒）
    /// </summary>
    public int SessionTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// 自动偏移量重置策略（earliest、latest、none）
    /// </summary>
    public string AutoOffsetReset { get; set; } = "earliest";

    /// <summary>
    /// 消息确认策略（0、1、-1/all）
    /// -1/all: 等待所有副本确认（最安全）
    /// 1: 等待 leader 确认
    /// 0: 不等待确认（最快但可能丢消息）
    /// </summary>
    public string Acks { get; set; } = "all";

    /// <summary>
    /// 重试次数
    /// </summary>
    public int Retries { get; set; } = 3;

    /// <summary>
    /// 批次大小（字节）
    /// </summary>
    public int BatchSize { get; set; } = 16384;

    /// <summary>
    /// 等待时间（毫秒）- 在发送批次前等待更多消息
    /// </summary>
    public int LingerMs { get; set; } = 10;

    /// <summary>
    /// 缓冲区内存大小（字节）
    /// </summary>
    public long BufferMemory { get; set; } = 33554432;

    /// <summary>
    /// 消息压缩类型（none、gzip、snappy、lz4、zstd）
    /// </summary>
    public string CompressionType { get; set; } = "none";

    /// <summary>
    /// SASL 机制（PLAIN、SCRAM-SHA-256、SCRAM-SHA-512）
    /// </summary>
    public string? SaslMechanism { get; set; }

    /// <summary>
    /// SASL 用户名
    /// </summary>
    public string? SaslUsername { get; set; }

    /// <summary>
    /// SASL 密码
    /// </summary>
    public string? SaslPassword { get; set; }

    /// <summary>
    /// 安全协议（PLAINTEXT、SSL、SASL_PLAINTEXT、SASL_SSL）
    /// </summary>
    public string SecurityProtocol { get; set; } = "PLAINTEXT";
}
