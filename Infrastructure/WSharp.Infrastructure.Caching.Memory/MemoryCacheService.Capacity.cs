using Microsoft.Extensions.Caching.Memory;

namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// MemoryCacheService 容量管理和统计扩展
/// </summary>
public partial class MemoryCacheService
{
    private readonly DateTime _startTime = DateTime.UtcNow;

    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    public MemoryCacheStatistics GetStatistics()
    {
        return new MemoryCacheStatistics
        {
            HitCount = _hitCount,
            MissCount = _missCount,
            CurrentEntryCount = _keyTracker?.Count ?? 0,
            EstimatedSize = GetEstimatedSize(),
            SizeLimit = _options.SizeLimit,
            CountLimit = _options.CountLimit,
            StartTime = _startTime
        };
    }

    /// <summary>
    /// 重置统计计数器
    /// </summary>
    public void ResetStatistics()
    {
        Interlocked.Exchange(ref _hitCount, 0);
        Interlocked.Exchange(ref _missCount, 0);
    }

    /// <summary>
    /// 获取估算的缓存大小（字节）
    /// </summary>
    /// <returns>估算的字节大小</returns>
    public long GetEstimatedSize()
    {
        if (_keyTracker == null)
        {
            return 0;
        }

        // 简单估算：假设每个键平均 50 字节，每个值平均 100 字节
        // 这是一个非常粗略的估算，实际大小取决于存储的对象类型
        const int averageKeySize = 50;
        const int averageValueSize = 100;

        return _keyTracker.Count * (averageKeySize + averageValueSize);
    }

    /// <summary>
    /// 获取当前缓存项数量
    /// </summary>
    public int GetCurrentCount()
    {
        return _keyTracker?.Count ?? 0;
    }

    /// <summary>
    /// 获取所有缓存键
    /// </summary>
    public IEnumerable<string> GetAllKeys()
    {
        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        return _keyTracker.GetAllKeys().Select(RemoveKeyPrefix);
    }

    /// <summary>
    /// 手动压缩缓存（触发内存压力清理）
    /// </summary>
    /// <param name="percentage">压缩百分比（0-1）</param>
    public void Compact(double percentage)
    {
        if (percentage < 0 || percentage > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 1.");
        }

        // IMemoryCache 支持 Compact 方法
        if (_cache is MemoryCache memoryCache)
        {
            memoryCache.Compact(percentage);
        }
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public void Clear()
    {
        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        var allKeys = _keyTracker.GetAllKeys().ToList();

        foreach (var key in allKeys)
        {
            _cache.Remove(key);
        }

        _keyTracker.Clear();
    }

    /// <summary>
    /// 检查缓存是否达到容量限制
    /// </summary>
    public bool IsAtCapacityLimit()
    {
        if (_keyTracker == null)
        {
            return false;
        }

        // 检查数量限制
        if (_options.CountLimit.HasValue && _keyTracker.Count >= _options.CountLimit.Value)
        {
            return true;
        }

        // 检查大小限制
        if (_options.SizeLimit.HasValue)
        {
            var estimatedSize = GetEstimatedSize();
            if (estimatedSize >= _options.SizeLimit.Value)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取缓存健康状态
    /// </summary>
    public CacheHealthStatus GetHealthStatus()
    {
        var stats = GetStatistics();
        var status = new CacheHealthStatus
        {
            IsHealthy = true,
            Timestamp = DateTime.UtcNow
        };

        // 检查命中率
        if (stats.TotalRequests > 100 && stats.HitRate < 50)
        {
            status.IsHealthy = false;
            status.Issues.Add("Low hit rate (below 50%)");
        }

        // 检查容量使用率
        if (stats.CountUsagePercentage.HasValue && stats.CountUsagePercentage.Value > 90)
        {
            status.IsHealthy = false;
            status.Issues.Add($"Entry count usage is high ({stats.CountUsagePercentage.Value:F2}%)");
        }

        if (stats.SizeUsagePercentage.HasValue && stats.SizeUsagePercentage.Value > 90)
        {
            status.IsHealthy = false;
            status.Issues.Add($"Size usage is high ({stats.SizeUsagePercentage.Value:F2}%)");
        }

        status.Statistics = stats;

        return status;
    }

    /// <summary>
    /// 移除最旧的缓存项（LRU 策略）
    /// </summary>
    /// <param name="count">要移除的数量</param>
    public int RemoveOldest(int count)
    {
        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        if (count <= 0)
        {
            return 0;
        }

        // IMemoryCache 不提供访问时间信息，我们只能随机移除
        // 在实际生产环境中，应该使用更复杂的 LRU 实现
        var keys = _keyTracker.GetAllKeys().Take(count).ToList();
        int removedCount = 0;

        foreach (var key in keys)
        {
            _cache.Remove(key);
            _keyTracker.RemoveKey(key);
            removedCount++;
        }

        return removedCount;
    }

    /// <summary>
    /// 根据优先级移除缓存项
    /// </summary>
    /// <param name="maxPriority">最大优先级（移除优先级低于或等于此值的项）</param>
    /// <returns>移除的数量</returns>
    public int RemoveByPriority(CacheItemPriority maxPriority)
    {
        // IMemoryCache 不提供按优先级查询的功能
        // 这个方法主要是作为 API 占位，实际实现需要更复杂的跟踪机制
        throw new NotSupportedException(
            "Removing by priority requires custom priority tracking which is not implemented in this version.");
    }
}

/// <summary>
/// 缓存健康状态
/// </summary>
public class CacheHealthStatus
{
    /// <summary>
    /// 是否健康
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// 检查时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 问题列表
    /// </summary>
    public List<string> Issues { get; set; } = new();

    /// <summary>
    /// 统计信息
    /// </summary>
    public MemoryCacheStatistics? Statistics { get; set; }

    /// <summary>
    /// 获取状态摘要
    /// </summary>
    public string GetSummary()
    {
        var status = IsHealthy ? "HEALTHY" : "UNHEALTHY";
        var summary = $"Cache Health: {status} (checked at {Timestamp:yyyy-MM-dd HH:mm:ss})";

        if (!IsHealthy && Issues.Count > 0)
        {
            summary += "\nIssues:\n- " + string.Join("\n- ", Issues);
        }

        return summary;
    }
}
