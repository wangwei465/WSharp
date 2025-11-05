using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace WSharp.Infrastructure.Caching;

/// <summary>
/// 分布式缓存服务
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly CacheOptions _options;

    /// <summary>
    /// 初始化分布式缓存服务
    /// </summary>
    /// <param name="cache">分布式缓存</param>
    /// <param name="options">缓存选项</param>
    public DistributedCacheService(IDistributedCache cache, IOptions<CacheOptions> options)
    {
        this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this._options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var bytes = await this._cache.GetAsync(fullKey, cancellationToken);

        if (bytes == null || bytes.Length == 0)
        {
            return default;
        }

        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(this._options.DefaultExpirationMinutes);

        var json = JsonConvert.SerializeObject(value);
        var bytes = Encoding.UTF8.GetBytes(json);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheExpiration
        };

        await this._cache.SetAsync(fullKey, bytes, options, cancellationToken);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        await this._cache.RemoveAsync(fullKey, cancellationToken);
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        var bytes = await this._cache.GetAsync(fullKey, cancellationToken);
        return bytes != null && bytes.Length > 0;
    }

    /// <summary>
    /// 刷新缓存过期时间
    /// </summary>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = this.BuildKey(key);
        await this._cache.RefreshAsync(fullKey, cancellationToken);
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
        var value = await this.GetAsync<T>(key, cancellationToken);

        if (value != null)
        {
            return value;
        }

        value = await factory();
        await this.SetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    /// <summary>
    /// 批量删除缓存（IDistributedCache 不直接支持，需要使用 Redis 特定命令）
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // IDistributedCache 接口不支持按模式删除
        // 如果使用 Redis，需要通过 ConnectionMultiplexer 直接操作
        throw new NotSupportedException("IDistributedCache 不支持按模式删除，请使用 RedisCacheService 或维护键列表");
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
