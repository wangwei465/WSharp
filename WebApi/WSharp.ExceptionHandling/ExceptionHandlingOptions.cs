namespace WSharp.ExceptionHandling;

/// <summary>
/// 异常处理配置选项
/// </summary>
public class ExceptionHandlingOptions
{
    /// <summary>
    /// 是否在生产环境中显示详细错误信息
    /// </summary>
    public bool ShowDetailedErrorsInProduction { get; set; } = false;

    /// <summary>
    /// 是否包含堆栈跟踪
    /// </summary>
    public bool IncludeStackTrace { get; set; } = false;

    /// <summary>
    /// 是否记录异常日志
    /// </summary>
    public bool LogExceptions { get; set; } = true;

    /// <summary>
    /// 是否使用中间件（否则使用过滤器）
    /// </summary>
    public bool UseMiddleware { get; set; } = true;

    /// <summary>
    /// 自定义错误消息映射
    /// </summary>
    public Dictionary<string, string> CustomErrorMessages { get; set; } = new();

    /// <summary>
    /// 需要忽略的异常类型（不记录日志）
    /// </summary>
    public List<Type> IgnoredExceptionTypes { get; set; } = new();
}
