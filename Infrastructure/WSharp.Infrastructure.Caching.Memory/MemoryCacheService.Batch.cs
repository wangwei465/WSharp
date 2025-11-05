using Microsoft.Extensions.Caching.Memory;

namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// MemoryCacheService 批量操作扩展
/// </summary>
public partial class MemoryCacheService
{
    /// <summary>
    /// 批量获取多个缓存项
    /// </summary>
    /// <typeparam name="T">缓存项类型</typeparam>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键值对字典</returns>
    public Task<Dictionary<string, T?>> GetManyAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = new Dictionary<string, T?>();
        var keyList = keys.ToList();

        foreach (var key in keyList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fullKey = BuildKey(key);
            if (_cache.TryGetValue(fullKey, out var objValue) && objValue is T value)
            {
                result[key] = value;

                if (_options.EnableStatistics)
                {
                    Interlocked.Increment(ref _hitCount);
                }
            }
            else
            {
                result[key] = default;

                if (_options.EnableStatistics)
                {
                    Interlocked.Increment(ref _missCount);
                }
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// 批量设置多个缓存项
    /// </summary>
    /// <typeparam name="T">缓存项类型</typeparam>
    /// <param name="items">键值对字典</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    public Task SetManyAsync<T>(
        Dictionary<string, T> items,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);

        foreach (var item in items)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fullKey = BuildKey(item.Key);
            var entryOptions = CreateCacheEntryOptions(cacheExpiration);

            // 设置移除回调
            if (_keyTracker != null)
            {
                entryOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _keyTracker.RemoveKey(key.ToString()!);
                });
            }

            _cache.Set(fullKey, item.Value, entryOptions);
            _keyTracker?.AddKey(fullKey);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 批量删除多个缓存项
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的键数量</returns>
    public Task<int> RemoveManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        int removedCount = 0;
        var keyList = keys.ToList();

        foreach (var key in keyList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fullKey = BuildKey(key);
            _cache.Remove(fullKey);
            _keyTracker?.RemoveKey(fullKey);
            removedCount++;
        }

        return Task.FromResult(removedCount);
    }

    /// <summary>
    /// 批量检查多个缓存项是否存在
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键存在状态字典</returns>
    public Task<Dictionary<string, bool>> ExistsManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = new Dictionary<string, bool>();
        var keyList = keys.ToList();

        foreach (var key in keyList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fullKey = BuildKey(key);
            result[key] = _cache.TryGetValue(fullKey, out _);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// 批量刷新多个缓存项的过期时间
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    public Task RefreshManyAsync(
        IEnumerable<string> keys,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
        var keyList = keys.ToList();

        foreach (var key in keyList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fullKey = BuildKey(key);

            // 重新获取并设置
            if (_cache.TryGetValue(fullKey, out var value))
            {
                var entryOptions = CreateCacheEntryOptions(cacheExpiration);

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
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 按前缀批量获取缓存项
    /// </summary>
    /// <typeparam name="T">缓存项类型</typeparam>
    /// <param name="prefix">键前缀</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键值对字典</returns>
    public Task<Dictionary<string, T?>> GetByPrefixAsync<T>(
        string prefix,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        var fullPrefix = BuildKey(prefix);
        var matchingKeys = _keyTracker.GetKeysByPrefix(fullPrefix);
        var result = new Dictionary<string, T?>();

        foreach (var fullKey in matchingKeys)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (_cache.TryGetValue<T>(fullKey, out var value))
            {
                // 移除前缀返回原始键
                var originalKey = RemoveKeyPrefix(fullKey);
                result[originalKey] = value;

                if (_options.EnableStatistics)
                {
                    Interlocked.Increment(ref _hitCount);
                }
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// 按前缀批量删除缓存项
    /// </summary>
    /// <param name="prefix">键前缀</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的键数量</returns>
    public Task<int> RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (_keyTracker == null)
        {
            throw new NotSupportedException(
                "Pattern matching is not enabled. Set EnablePatternMatching to true in MemoryCacheOptions.");
        }

        var fullPrefix = BuildKey(prefix);
        var matchingKeys = _keyTracker.GetKeysByPrefix(fullPrefix);
        int removedCount = 0;

        foreach (var key in matchingKeys)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _cache.Remove(key);
            _keyTracker.RemoveKey(key);
            removedCount++;
        }

        return Task.FromResult(removedCount);
    }

    /// <summary>
    /// 移除键前缀
    /// </summary>
    private string RemoveKeyPrefix(string fullKey)
    {
        if (string.IsNullOrEmpty(_options.KeyPrefix))
        {
            return fullKey;
        }

        var prefixWithColon = $"{_options.KeyPrefix}:";
        return fullKey.StartsWith(prefixWithColon)
            ? fullKey[prefixWithColon.Length..]
            : fullKey;
    }
}
