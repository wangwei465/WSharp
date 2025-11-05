using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis Lua 脚本服务
/// </summary>
public class RedisScriptService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly RedisCacheOptions _options;
    private readonly ConcurrentDictionary<string, LuaScript> _scriptCache;

    public RedisScriptService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisCacheOptions> options)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _database = _connectionMultiplexer.GetDatabase(_options.Database);
        _scriptCache = new ConcurrentDictionary<string, LuaScript>();
    }

    /// <summary>
    /// 执行 Lua 脚本
    /// </summary>
    /// <param name="script">Lua 脚本内容</param>
    /// <param name="keys">Redis 键列表</param>
    /// <param name="values">参数值列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>脚本执行结果</returns>
    public async Task<RedisResult> ExecuteScriptAsync(
        string script,
        RedisKey[]? keys = null,
        RedisValue[]? values = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(script);

        // 尝试从缓存中获取已准备的脚本
        var preparedScript = _scriptCache.GetOrAdd(script, s => LuaScript.Prepare(s));

        return await _database.ScriptEvaluateAsync(
            preparedScript,
            new { keys = keys ?? Array.Empty<RedisKey>(), values = values ?? Array.Empty<RedisValue>() }
        );
    }

    /// <summary>
    /// 执行 Lua 脚本并返回字符串结果
    /// </summary>
    public async Task<string?> ExecuteScriptAsStringAsync(
        string script,
        RedisKey[]? keys = null,
        RedisValue[]? values = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteScriptAsync(script, keys, values, cancellationToken);
        return result.IsNull ? null : result.ToString();
    }

    #region 内置脚本

    /// <summary>
    /// 条件设置：仅当键不存在时设置值（SETNX）
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expirationSeconds">过期时间（秒）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否设置成功</returns>
    public async Task<bool> SetIfNotExistsAsync(
        string key,
        string value,
        int expirationSeconds,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            if redis.call('EXISTS', KEYS[1]) == 0 then
                redis.call('SETEX', KEYS[1], ARGV[2], ARGV[1])
                return 1
            else
                return 0
            end
        ";

        var fullKey = BuildKey(key);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)value, (RedisValue)expirationSeconds },
            cancellationToken
        );

        return (int)result == 1;
    }

    /// <summary>
    /// 原子性比较并设置：仅当当前值等于预期值时才更新（CAS - Compare And Set）
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="expectedValue">预期值</param>
    /// <param name="newValue">新值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否设置成功</returns>
    public async Task<bool> CompareAndSetAsync(
        string key,
        string expectedValue,
        string newValue,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            local current = redis.call('GET', KEYS[1])
            if current == ARGV[1] then
                redis.call('SET', KEYS[1], ARGV[2])
                return 1
            else
                return 0
            end
        ";

        var fullKey = BuildKey(key);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)expectedValue, (RedisValue)newValue },
            cancellationToken
        );

        return (int)result == 1;
    }

    /// <summary>
    /// 有界递增：递增但不超过最大值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="incrementBy">递增值</param>
    /// <param name="maxValue">最大值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>递增后的值，如果超过最大值则返回 -1</returns>
    public async Task<long> BoundedIncrementAsync(
        string key,
        long incrementBy,
        long maxValue,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            local current = tonumber(redis.call('GET', KEYS[1]) or '0')
            local newValue = current + tonumber(ARGV[1])
            local maxValue = tonumber(ARGV[2])

            if newValue <= maxValue then
                redis.call('SET', KEYS[1], newValue)
                return newValue
            else
                return -1
            end
        ";

        var fullKey = BuildKey(key);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)incrementBy, (RedisValue)maxValue },
            cancellationToken
        );

        return (long)result;
    }

    /// <summary>
    /// 获取并删除：原子性获取值并删除键
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键的值，如果不存在则返回 null</returns>
    public async Task<string?> GetAndDeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            local value = redis.call('GET', KEYS[1])
            if value then
                redis.call('DEL', KEYS[1])
            end
            return value
        ";

        var fullKey = BuildKey(key);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            null,
            cancellationToken
        );

        return result.IsNull ? null : result.ToString();
    }

    /// <summary>
    /// 限流脚本：滑动窗口限流
    /// </summary>
    /// <param name="key">限流键</param>
    /// <param name="limit">限流阈值</param>
    /// <param name="windowSeconds">时间窗口（秒）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否允许通过</returns>
    public async Task<bool> RateLimitAsync(
        string key,
        int limit,
        int windowSeconds,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            local current = redis.call('GET', KEYS[1])
            if current and tonumber(current) >= tonumber(ARGV[1]) then
                return 0
            else
                local count = redis.call('INCR', KEYS[1])
                if count == 1 then
                    redis.call('EXPIRE', KEYS[1], ARGV[2])
                end
                return 1
            end
        ";

        var fullKey = BuildKey(key);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)limit, (RedisValue)windowSeconds },
            cancellationToken
        );

        return (int)result == 1;
    }

    /// <summary>
    /// 分布式锁获取
    /// </summary>
    /// <param name="lockKey">锁键</param>
    /// <param name="lockValue">锁值（用于解锁验证）</param>
    /// <param name="expirationSeconds">锁过期时间（秒）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否获取锁成功</returns>
    public async Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        int expirationSeconds,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            if redis.call('EXISTS', KEYS[1]) == 0 then
                redis.call('SETEX', KEYS[1], ARGV[2], ARGV[1])
                return 1
            else
                return 0
            end
        ";

        var fullKey = BuildKey(lockKey);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)lockValue, (RedisValue)expirationSeconds },
            cancellationToken
        );

        return (int)result == 1;
    }

    /// <summary>
    /// 分布式锁释放
    /// </summary>
    /// <param name="lockKey">锁键</param>
    /// <param name="lockValue">锁值（用于验证）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否释放锁成功</returns>
    public async Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue,
        CancellationToken cancellationToken = default)
    {
        const string script = @"
            if redis.call('GET', KEYS[1]) == ARGV[1] then
                redis.call('DEL', KEYS[1])
                return 1
            else
                return 0
            end
        ";

        var fullKey = BuildKey(lockKey);
        var result = await ExecuteScriptAsync(
            script,
            new[] { (RedisKey)fullKey },
            new[] { (RedisValue)lockValue },
            cancellationToken
        );

        return (int)result == 1;
    }

    #endregion

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
    /// 清除脚本缓存
    /// </summary>
    public void ClearScriptCache()
    {
        _scriptCache.Clear();
    }

    #endregion
}
