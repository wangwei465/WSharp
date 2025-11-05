using StackExchange.Redis;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// RedisCacheService 批量操作扩展
/// </summary>
public partial class RedisCacheService
{
    /// <summary>
    /// 批量获取多个缓存项
    /// </summary>
    /// <typeparam name="T">缓存项类型</typeparam>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键值对字典</returns>
    public async Task<Dictionary<string, T?>> GetManyAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var keyList = keys.ToList();
        if (keyList.Count == 0)
        {
            return new Dictionary<string, T?>();
        }

        // 构建完整的 Redis 键
        var redisKeys = keyList.Select(k => (RedisKey)BuildKey(k)).ToArray();

        // 批量获取
        var values = await _database.StringGetAsync(redisKeys);

        // 构建结果字典
        var result = new Dictionary<string, T?>();
        for (int i = 0; i < keyList.Count; i++)
        {
            var value = values[i];
            result[keyList[i]] = value.HasValue ? Deserialize<T>(value!) : default;
        }

        return result;
    }

    /// <summary>
    /// 批量设置多个缓存项
    /// </summary>
    /// <typeparam name="T">缓存项类型</typeparam>
    /// <param name="items">键值对字典</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task SetManyAsync<T>(
        Dictionary<string, T> items,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
        {
            return;
        }

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
        var batch = _database.CreateBatch();
        var tasks = new List<Task>();

        foreach (var item in items)
        {
            var fullKey = BuildKey(item.Key);
            var serializedValue = Serialize(item.Value);
            tasks.Add(batch.StringSetAsync(fullKey, serializedValue, cacheExpiration));
        }

        batch.Execute();
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 批量删除多个缓存项
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的键数量</returns>
    public async Task<long> RemoveManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var keyList = keys.ToList();
        if (keyList.Count == 0)
        {
            return 0;
        }

        // 构建完整的 Redis 键
        var redisKeys = keyList.Select(k => (RedisKey)BuildKey(k)).ToArray();

        // 批量删除
        return await _database.KeyDeleteAsync(redisKeys);
    }

    /// <summary>
    /// 批量检查多个缓存项是否存在
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键存在状态字典</returns>
    public async Task<Dictionary<string, bool>> ExistsManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var keyList = keys.ToList();
        if (keyList.Count == 0)
        {
            return new Dictionary<string, bool>();
        }

        var batch = _database.CreateBatch();
        var tasks = new Dictionary<string, Task<bool>>();

        foreach (var key in keyList)
        {
            var fullKey = BuildKey(key);
            tasks[key] = batch.KeyExistsAsync(fullKey);
        }

        batch.Execute();
        await Task.WhenAll(tasks.Values);

        // 构建结果字典
        return tasks.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Result);
    }

    /// <summary>
    /// 批量刷新多个缓存项的过期时间
    /// </summary>
    /// <param name="keys">键列表</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task RefreshManyAsync(
        IEnumerable<string> keys,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var keyList = keys.ToList();
        if (keyList.Count == 0)
        {
            return;
        }

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
        var batch = _database.CreateBatch();
        var tasks = new List<Task>();

        foreach (var key in keyList)
        {
            var fullKey = BuildKey(key);
            tasks.Add(batch.KeyExpireAsync(fullKey, cacheExpiration));
        }

        batch.Execute();
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 原子性递增
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">递增值（默认为 1）</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>递增后的值</returns>
    public async Task<long> IncrementAsync(
        string key,
        long value = 1,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        var result = await _database.StringIncrementAsync(fullKey, value);

        // 如果指定了过期时间，则设置过期时间
        if (expiration.HasValue)
        {
            await _database.KeyExpireAsync(fullKey, expiration.Value);
        }

        return result;
    }

    /// <summary>
    /// 原子性递减
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">递减值（默认为 1）</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>递减后的值</returns>
    public async Task<long> DecrementAsync(
        string key,
        long value = 1,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        var result = await _database.StringDecrementAsync(fullKey, value);

        // 如果指定了过期时间，则设置过期时间
        if (expiration.HasValue)
        {
            await _database.KeyExpireAsync(fullKey, expiration.Value);
        }

        return result;
    }
}
