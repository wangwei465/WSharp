namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// 内存缓存配置选项
/// </summary>
public class MemoryCacheOptions : CacheOptions
{
    /// <summary>
    /// 缓存大小限制（字节数，null 表示无限制）
    /// </summary>
    public long? SizeLimit { get; set; }

    /// <summary>
    /// 缓存条目数量限制（null 表示无限制）
    /// </summary>
    public int? CountLimit { get; set; }

    /// <summary>
    /// 压缩策略百分比（0-1），当缓存使用率达到此比例时触发压缩
    /// </summary>
    public double CompactionPercentage { get; set; } = 0.05;

    /// <summary>
    /// 过期扫描频率（分钟）
    /// </summary>
    public int ExpirationScanFrequencyMinutes { get; set; } = 1;

    /// <summary>
    /// 是否启用统计功能
    /// </summary>
    public bool EnableStatistics { get; set; } = false;

    /// <summary>
    /// 是否启用模式匹配删除功能（需要跟踪所有键）
    /// </summary>
    public bool EnablePatternMatching { get; set; } = true;

    /// <summary>
    /// 是否在设置缓存时跟踪缓存大小
    /// </summary>
    public bool TrackSize { get; set; } = false;

    /// <summary>
    /// 滑动过期时间（分钟），如果设置则使用滑动过期策略
    /// </summary>
    public int? SlidingExpirationMinutes { get; set; }

    /// <summary>
    /// 绝对过期时间（分钟），相对于当前时间
    /// </summary>
    public int? AbsoluteExpirationMinutes { get; set; }

    /// <summary>
    /// 缓存优先级
    /// </summary>
    public CacheItemPriority DefaultPriority { get; set; } = CacheItemPriority.Normal;
}

/// <summary>
/// 缓存项优先级
/// </summary>
public enum CacheItemPriority
{
    /// <summary>
    /// 低优先级，在内存压力下首先被移除
    /// </summary>
    Low = 0,

    /// <summary>
    /// 正常优先级
    /// </summary>
    Normal = 1,

    /// <summary>
    /// 高优先级，在内存压力下最后被移除
    /// </summary>
    High = 2,

    /// <summary>
    /// 永不移除（除非手动删除或过期）
    /// </summary>
    NeverRemove = 3
}
