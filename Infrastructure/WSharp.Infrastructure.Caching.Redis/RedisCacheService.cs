using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 缓存服务实现
/// </summary>
public partial class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly RedisCacheOptions _options;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisCacheOptions> options)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _database = _connectionMultiplexer.GetDatabase(_options.Database);
    }

    /// <summary>
    /// 获取缓存项
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        var value = await _database.StringGetAsync(fullKey);

        if (!value.HasValue)
        {
            return default;
        }

        return Deserialize<T>(value!);
    }

    /// <summary>
    /// 设置缓存项
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var fullKey = BuildKey(key);
        var serializedValue = Serialize(value);
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);

        await _database.StringSetAsync(fullKey, serializedValue, cacheExpiration);
    }

    /// <summary>
    /// 删除缓存项
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        await _database.KeyDeleteAsync(fullKey);
    }

    /// <summary>
    /// 检查缓存项是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        return await _database.KeyExistsAsync(fullKey);
    }

    /// <summary>
    /// 刷新缓存项过期时间
    /// </summary>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var fullKey = BuildKey(key);
        var exists = await _database.KeyExistsAsync(fullKey);

        if (exists)
        {
            var expiration = TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
            await _database.KeyExpireAsync(fullKey, expiration);
        }
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

        // 先尝试获取
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        // 不存在则创建
        var value = await factory();
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }

    /// <summary>
    /// 按模式删除缓存项（使用 SCAN 命令）
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var fullPattern = BuildKey(pattern);
        var server = GetServer();

        // 使用 SCAN 命令安全地遍历键
        var keys = server.KeysAsync(
            database: _options.Database,
            pattern: fullPattern,
            pageSize: _options.ScanPageSize);

        var batch = _database.CreateBatch();
        var tasks = new List<Task>();

        await foreach (var key in keys)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            tasks.Add(batch.KeyDeleteAsync(key));

            // 每 100 个键执行一次批处理
            if (tasks.Count >= 100)
            {
                batch.Execute();
                await Task.WhenAll(tasks);
                tasks.Clear();
                batch = _database.CreateBatch();
            }
        }

        // 执行剩余的删除操作
        if (tasks.Count > 0)
        {
            batch.Execute();
            await Task.WhenAll(tasks);
        }
    }

    #region 辅助方法

    /// <summary>
    /// 构建完整的缓存键
    /// </summary>
    private string BuildKey(string key)
    {
        var prefix = !string.IsNullOrEmpty(_options.InstanceName)
            ? _options.InstanceName
            : _options.KeyPrefix;

        return string.IsNullOrEmpty(prefix)
            ? key
            : $"{prefix}:{key}";
    }

    /// <summary>
    /// 序列化对象为字符串
    /// </summary>
    private string Serialize<T>(T value)
    {
        return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// 反序列化字符串为对象
    /// </summary>
    private T? Deserialize<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// 获取 Redis 服务器实例
    /// </summary>
    private IServer GetServer()
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        if (endpoints.Length == 0)
        {
            throw new InvalidOperationException("No Redis endpoints available.");
        }

        return _connectionMultiplexer.GetServer(endpoints[0]);
    }

    #endregion
}
