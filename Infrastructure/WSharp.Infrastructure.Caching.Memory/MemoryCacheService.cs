using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// 内存缓存服务实现
/// </summary>
public partial class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheOptions _options;
    private readonly MemoryKeyTracker? _keyTracker;
    private long _hitCount;
    private long _missCount;

    public MemoryCacheService(
        IMemoryCache cache,
        IOptions<MemoryCacheOptions> options)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // 如果启用模式匹配，初始化键跟踪器
        if (_options.EnablePatternMatching)
        {
            _keyTracker = new MemoryKeyTracker();
        }
    }

    /// <summary>
    /// 获取缓存项
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);

        if (_cache.TryGetValue<T>(fullKey, out var value))
        {
            if (_options.EnableStatistics)
            {
                Interlocked.Increment(ref _hitCount);
            }
            return Task.FromResult<T?>(value);
        }

        if (_options.EnableStatistics)
        {
            Interlocked.Increment(ref _missCount);
        }

        return Task.FromResult<T?>(default);
    }

    /// <summary>
    /// 设置缓存项
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var fullKey = BuildKey(key);
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);

        var entryOptions = CreateCacheEntryOptions(cacheExpiration);

        // 设置缓存项移除回调，用于从键跟踪器中移除
        if (_keyTracker != null)
        {
            entryOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _keyTracker.RemoveKey(key.ToString()!);
            });
        }

        _cache.Set(fullKey, value, entryOptions);

        // 添加到键跟踪器
        _keyTracker?.AddKey(fullKey);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 删除缓存项
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        _cache.Remove(fullKey);
        _keyTracker?.RemoveKey(fullKey);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 检查缓存项是否存在
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        var exists = _cache.TryGetValue(fullKey, out _);

        return Task.FromResult(exists);
    }

    /// <summary>
    /// 刷新缓存项过期时间
    /// </summary>
    public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);

        // IMemoryCache 不支持直接刷新过期时间
        // 需要重新获取值并重新设置
        if (_cache.TryGetValue(fullKey, out var value))
        {
            var expiration = TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
            var entryOptions = CreateCacheEntryOptions(expiration);

            // 设置移除回调
            if (_keyTracker != null)
            {
                entryOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _keyTracker.RemoveKey(key.ToString()!);
                });
            }

            _cache.Set(fullKey, value, entryOptions);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取或创建缓存项
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(factory);

        var fullKey = BuildKey(key);

        // 尝试获取现有缓存
        if (_cache.TryGetValue<T>(fullKey, out var cachedValue))
        {
            if (_options.EnableStatistics)
            {
                Interlocked.Increment(ref _hitCount);
            }
            return cachedValue!;
        }

        if (_options.EnableStatistics)
        {
            Interlocked.Increment(ref _missCount);
        }

        // 创建新值
        var value = await factory();

        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }

    /// <summary>
    /// 按模式删除缓存项
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        var fullPattern = BuildKey(pattern);
        var matchingKeys = _keyTracker.GetKeysByPattern(fullPattern);

        foreach (var key in matchingKeys)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _cache.Remove(key);
            _keyTracker.RemoveKey(key);
        }

        return Task.CompletedTask;
    }

    #region 辅助方法

    /// <summary>
    /// 构建完整的缓存键
    /// </summary>
    private string BuildKey(string key)
    {
        return string.IsNullOrEmpty(_options.KeyPrefix)
            ? key
            : $"{_options.KeyPrefix}:{key}";
    }

    /// <summary>
    /// 创建缓存条目选项
    /// </summary>
    private MemoryCacheEntryOptions CreateCacheEntryOptions(TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions();

        // 设置过期策略
        if (_options.SlidingExpirationMinutes.HasValue)
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(_options.SlidingExpirationMinutes.Value);
        }

        if (_options.AbsoluteExpirationMinutes.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.AbsoluteExpirationMinutes.Value);
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }

        // 设置优先级
        options.Priority = ConvertPriority(_options.DefaultPriority);

        // 如果启用大小跟踪
        if (_options.TrackSize && _options.SizeLimit.HasValue)
        {
            options.Size = 1; // 简单计数，每个条目占 1
        }

        return options;
    }

    /// <summary>
    /// 转换优先级枚举
    /// </summary>
    private static Microsoft.Extensions.Caching.Memory.CacheItemPriority ConvertPriority(CacheItemPriority priority)
    {
        return priority switch
        {
            CacheItemPriority.Low => Microsoft.Extensions.Caching.Memory.CacheItemPriority.Low,
            CacheItemPriority.Normal => Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal,
            CacheItemPriority.High => Microsoft.Extensions.Caching.Memory.CacheItemPriority.High,
            CacheItemPriority.NeverRemove => Microsoft.Extensions.Caching.Memory.CacheItemPriority.NeverRemove,
            _ => Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal
        };
    }

    #endregion
}
