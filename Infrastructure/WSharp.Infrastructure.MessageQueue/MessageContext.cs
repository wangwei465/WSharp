namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息上下文
/// </summary>
public class MessageContext
{
    /// <summary>
    /// 消息 ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// 相关 ID（用于关联请求）
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 消息时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 消息来源
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// 消息目标
    /// </summary>
    public string? Destination { get; set; }

    /// <summary>
    /// 其他属性
    /// </summary>
    public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// 消息处理结果
/// </summary>
public class MessageHandlerResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 是否需要重试
    /// </summary>
    public bool ShouldRetry { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static MessageHandlerResult Successful() => new() { Success = true };

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static MessageHandlerResult Failed(string errorMessage, bool shouldRetry = false) =>
        new() { Success = false, ErrorMessage = errorMessage, ShouldRetry = shouldRetry };
}
