using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WSharp.Infrastructure.Caching;

/// <summary>
/// 内存缓存服务
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _options;

    /// <summary>
    /// 初始化内存缓存服务
    /// </summary>
    /// <param name="cache">内存缓存</param>
    /// <param name="options">缓存选项</param>
    public MemoryCacheService(IMemoryCache cache, IOptions<CacheOptions> options)
    {
        this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this._options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var value = this._cache.Get<T>(fullKey);
        return Task.FromResult(value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(this._options.DefaultExpirationMinutes);

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheExpiration
        };

        this._cache.Set(fullKey, value, cacheEntryOptions);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        this._cache.Remove(fullKey);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var exists = this._cache.TryGetValue(fullKey, out _);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// 刷新缓存过期时间
    /// </summary>
    public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // IMemoryCache 没有直接的 Refresh 方法
        // 通过重新获取值来触发滑动过期时间的更新
        if (this._cache.TryGetValue(fullKey, out var value))
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(this._options.DefaultExpirationMinutes)
            };
            this._cache.Set(fullKey, value, cacheEntryOptions);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取或创建缓存
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(this._options.DefaultExpirationMinutes);

        return await this._cache.GetOrCreateAsync(fullKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = cacheExpiration;
            return await factory();
        }) ?? default!;
    }

    /// <summary>
    /// 批量删除缓存（内存缓存不支持模式匹配，需要维护键列表）
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // IMemoryCache 不支持按模式删除
        // 需要应用层维护键列表或使用第三方库
        throw new NotSupportedException("内存缓存不支持按模式删除，请使用分布式缓存或维护键列表");
    }

    /// <summary>
    /// 构建完整缓存键
    /// </summary>
    private string BuildKey(string key)
    {
        return string.IsNullOrEmpty(this._options.KeyPrefix)
            ? key
            : $"{this._options.KeyPrefix}:{key}";
    }
}
