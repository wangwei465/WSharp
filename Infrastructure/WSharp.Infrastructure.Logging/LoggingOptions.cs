namespace WSharp.Infrastructure.Logging;

/// <summary>
/// 日志配置选项
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    public string ApplicationName { get; set; } = "WSharp";

    /// <summary>
    /// 日志级别
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// 是否写入控制台
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// 是否写入文件
    /// </summary>
    public bool WriteToFile { get; set; } = true;

    /// <summary>
    /// 文件路径
    /// </summary>
    public string FilePath { get; set; } = "logs/log-.txt";

    /// <summary>
    /// 文件滚动间隔
    /// </summary>
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>
    /// 文件保留天数
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// 是否写入 Elasticsearch
    /// </summary>
    public bool WriteToElasticsearch { get; set; }

    /// <summary>
    /// Elasticsearch URI
    /// </summary>
    public string? ElasticsearchUri { get; set; }

    /// <summary>
    /// Elasticsearch 索引格式
    /// </summary>
    public string ElasticsearchIndexFormat { get; set; } = "wsharp-logs-{0:yyyy.MM.dd}";

    /// <summary>
    /// 是否启用结构化日志
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = true;

    /// <summary>
    /// 是否启用请求日志
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;
}

/// <summary>
/// 文件滚动间隔
/// </summary>
public enum RollingInterval
{
    /// <summary>
    /// 每秒
    /// </summary>
    Second,

    /// <summary>
    /// 每分钟
    /// </summary>
    Minute,

    /// <summary>
    /// 每小时
    /// </summary>
    Hour,

    /// <summary>
    /// 每天
    /// </summary>
    Day,

    /// <summary>
    /// 每月
    /// </summary>
    Month,

    /// <summary>
    /// 每年
    /// </summary>
    Year
}
