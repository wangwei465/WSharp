namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// 内存缓存统计信息
/// </summary>
public class MemoryCacheStatistics
{
    /// <summary>
    /// 缓存命中次数
    /// </summary>
    public long HitCount { get; set; }

    /// <summary>
    /// 缓存未命中次数
    /// </summary>
    public long MissCount { get; set; }

    /// <summary>
    /// 总请求次数
    /// </summary>
    public long TotalRequests => HitCount + MissCount;

    /// <summary>
    /// 缓存命中率（百分比）
    /// </summary>
    public double HitRate => TotalRequests > 0 ? (double)HitCount / TotalRequests * 100 : 0;

    /// <summary>
    /// 缓存未命中率（百分比）
    /// </summary>
    public double MissRate => TotalRequests > 0 ? (double)MissCount / TotalRequests * 100 : 0;

    /// <summary>
    /// 当前缓存项数量
    /// </summary>
    public int CurrentEntryCount { get; set; }

    /// <summary>
    /// 估算的缓存大小（字节）
    /// </summary>
    public long EstimatedSize { get; set; }

    /// <summary>
    /// 配置的大小限制（字节，null 表示无限制）
    /// </summary>
    public long? SizeLimit { get; set; }

    /// <summary>
    /// 大小使用率（百分比，如果有限制）
    /// </summary>
    public double? SizeUsagePercentage => SizeLimit.HasValue && SizeLimit.Value > 0
        ? (double)EstimatedSize / SizeLimit.Value * 100
        : null;

    /// <summary>
    /// 配置的数量限制（null 表示无限制）
    /// </summary>
    public int? CountLimit { get; set; }

    /// <summary>
    /// 数量使用率（百分比，如果有限制）
    /// </summary>
    public double? CountUsagePercentage => CountLimit.HasValue && CountLimit.Value > 0
        ? (double)CurrentEntryCount / CountLimit.Value * 100
        : null;

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 统计持续时间
    /// </summary>
    public TimeSpan Duration => DateTime.UtcNow - StartTime;

    /// <summary>
    /// 每秒请求数
    /// </summary>
    public double RequestsPerSecond => Duration.TotalSeconds > 0
        ? TotalRequests / Duration.TotalSeconds
        : 0;

    /// <summary>
    /// 每秒命中数
    /// </summary>
    public double HitsPerSecond => Duration.TotalSeconds > 0
        ? HitCount / Duration.TotalSeconds
        : 0;

    /// <summary>
    /// 每秒未命中数
    /// </summary>
    public double MissesPerSecond => Duration.TotalSeconds > 0
        ? MissCount / Duration.TotalSeconds
        : 0;

    /// <summary>
    /// 重置统计信息
    /// </summary>
    public void Reset()
    {
        HitCount = 0;
        MissCount = 0;
        CurrentEntryCount = 0;
        EstimatedSize = 0;
        StartTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取统计摘要
    /// </summary>
    public string GetSummary()
    {
        return $@"Memory Cache Statistics:
- Total Requests: {TotalRequests:N0}
- Hit Count: {HitCount:N0} ({HitRate:F2}%)
- Miss Count: {MissCount:N0} ({MissRate:F2}%)
- Current Entries: {CurrentEntryCount:N0}{(CountLimit.HasValue ? $"/{CountLimit.Value:N0}" : "")}
- Estimated Size: {FormatBytes(EstimatedSize)}{(SizeLimit.HasValue ? $"/{FormatBytes(SizeLimit.Value)}" : "")}
- Requests/sec: {RequestsPerSecond:F2}
- Hits/sec: {HitsPerSecond:F2}
- Misses/sec: {MissesPerSecond:F2}
- Duration: {Duration:g}";
    }

    /// <summary>
    /// 格式化字节数
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
