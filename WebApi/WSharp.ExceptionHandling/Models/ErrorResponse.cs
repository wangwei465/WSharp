namespace WSharp.ExceptionHandling.Models;

/// <summary>
/// 错误响应模型
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 详细错误信息（仅开发环境）
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// 堆栈跟踪（仅开发环境）
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// 验证错误
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public Dictionary<string, object>? AdditionalData { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 跟踪 ID
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string? Path { get; set; }
}
