using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WSharp.Infrastructure.Caching;

/// <summary>
/// 混合缓存服务（多级缓存）
/// L1: 内存缓存
/// L2: 分布式缓存
/// </summary>
public class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly HybridCacheOptions _options;

    /// <summary>
    /// 初始化混合缓存服务
    /// </summary>
    /// <param name="memoryCache">内存缓存</param>
    /// <param name="distributedCache">分布式缓存</param>
    /// <param name="options">混合缓存选项</param>
    public HybridCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IOptions<HybridCacheOptions> options)
    {
        this._memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        this._distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this._options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// 获取缓存（优先从 L1 获取，不存在则从 L2 获取并回填 L1）
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // L1: 内存缓存
        if (this._options.EnableL1Cache && this._memoryCache.TryGetValue<T>(fullKey, out var l1Value))
        {
            return l1Value;
        }

        // L2: 分布式缓存
        if (this._options.EnableL2Cache)
        {
            var bytes = await this._distributedCache.GetAsync(fullKey, cancellationToken);
            if (bytes != null && bytes.Length > 0)
            {
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                var l2Value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

                // 回填 L1 缓存
                if (this._options.EnableL1Cache && l2Value != null)
                {
                    var l1Options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(this._options.L1ExpirationMinutes)
                    };
                    this._memoryCache.Set(fullKey, l2Value, l1Options);
                }

                return l2Value;
            }
        }

        return default;
    }

    /// <summary>
    /// 设置缓存（同时写入 L1 和 L2）
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // L1: 内存缓存
        if (this._options.EnableL1Cache)
        {
            var l1Expiration = expiration ?? TimeSpan.FromMinutes(this._options.L1ExpirationMinutes);
            var l1Options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = l1Expiration
            };
            this._memoryCache.Set(fullKey, value, l1Options);
        }

        // L2: 分布式缓存
        if (this._options.EnableL2Cache)
        {
            var l2Expiration = expiration ?? TimeSpan.FromMinutes(this._options.L2ExpirationMinutes);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            var l2Options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = l2Expiration
            };

            await this._distributedCache.SetAsync(fullKey, bytes, l2Options, cancellationToken);
        }
    }

    /// <summary>
    /// 删除缓存（同时删除 L1 和 L2）
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // L1: 内存缓存
        if (this._options.EnableL1Cache)
        {
            this._memoryCache.Remove(fullKey);
        }

        // L2: 分布式缓存
        if (this._options.EnableL2Cache)
        {
            await this._distributedCache.RemoveAsync(fullKey, cancellationToken);
        }
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // L1: 内存缓存
        if (this._options.EnableL1Cache && this._memoryCache.TryGetValue(fullKey, out _))
        {
            return true;
        }

        // L2: 分布式缓存
        if (this._options.EnableL2Cache)
        {
            var bytes = await this._distributedCache.GetAsync(fullKey, cancellationToken);
            return bytes != null && bytes.Length > 0;
        }

        return false;
    }

    /// <summary>
    /// 刷新缓存过期时间
    /// </summary>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);

        // L1: 内存缓存刷新
        if (this._options.EnableL1Cache && this._memoryCache.TryGetValue(fullKey, out var value))
        {
            var l1Options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(this._options.L1ExpirationMinutes)
            };
            this._memoryCache.Set(fullKey, value, l1Options);
        }

        // L2: 分布式缓存刷新
        if (this._options.EnableL2Cache)
        {
            await this._distributedCache.RefreshAsync(fullKey, cancellationToken);
        }
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
        // 先尝试获取
        var value = await this.GetAsync<T>(key, cancellationToken);

        if (value != null)
        {
            return value;
        }

        // 不存在则创建
        value = await factory();
        await this.SetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    /// <summary>
    /// 批量删除缓存
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // 混合缓存不支持按���式删除
        throw new NotSupportedException("混合缓存不支持按模式删除，请使用 RedisCacheService 或维护键列表");
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
