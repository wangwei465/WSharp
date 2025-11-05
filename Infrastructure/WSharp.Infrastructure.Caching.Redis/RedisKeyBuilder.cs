namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 键构建工具类
/// </summary>
public static class RedisKeyBuilder
{
    /// <summary>
    /// 构建带前缀的完整键
    /// </summary>
    /// <param name="key">原始键</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildKey(string key, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(key);

        return string.IsNullOrEmpty(prefix)
            ? key
            : $"{prefix}:{key}";
    }

    /// <summary>
    /// 构建多级键
    /// </summary>
    /// <param name="parts">键的各个部分</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildKey(params string[] parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        if (parts.Length == 0)
        {
            throw new ArgumentException("At least one key part is required.", nameof(parts));
        }

        return string.Join(":", parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    /// <summary>
    /// 构建用户相关的键
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="key">键名</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildUserKey(string userId, string key, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(key);

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(prefix))
        {
            parts.Add(prefix);
        }
        parts.Add("user");
        parts.Add(userId);
        parts.Add(key);

        return string.Join(":", parts);
    }

    /// <summary>
    /// 构建会话相关的键
    /// </summary>
    /// <param name="sessionId">会话 ID</param>
    /// <param name="key">键名</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildSessionKey(string sessionId, string key, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(sessionId);
        ArgumentNullException.ThrowIfNull(key);

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(prefix))
        {
            parts.Add(prefix);
        }
        parts.Add("session");
        parts.Add(sessionId);
        parts.Add(key);

        return string.Join(":", parts);
    }

    /// <summary>
    /// 构建缓存键（带环境和版本）
    /// </summary>
    /// <param name="environment">环境名称（如 dev, staging, prod）</param>
    /// <param name="version">版本号</param>
    /// <param name="key">键名</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildCacheKey(string environment, string version, string key, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(key);

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(prefix))
        {
            parts.Add(prefix);
        }
        parts.Add("cache");
        parts.Add(environment);
        parts.Add(version);
        parts.Add(key);

        return string.Join(":", parts);
    }

    /// <summary>
    /// 构建锁键
    /// </summary>
    /// <param name="resource">资源名称</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildLockKey(string resource, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(prefix))
        {
            parts.Add(prefix);
        }
        parts.Add("lock");
        parts.Add(resource);

        return string.Join(":", parts);
    }

    /// <summary>
    /// 构建限流键
    /// </summary>
    /// <param name="identifier">标识符（如用户 ID、IP 地址等）</param>
    /// <param name="action">操作名称</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 键</returns>
    public static string BuildRateLimitKey(string identifier, string action, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        ArgumentNullException.ThrowIfNull(action);

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(prefix))
        {
            parts.Add(prefix);
        }
        parts.Add("ratelimit");
        parts.Add(action);
        parts.Add(identifier);

        return string.Join(":", parts);
    }

    /// <summary>
    /// 从完整键中移除前缀
    /// </summary>
    /// <param name="fullKey">完整的 Redis 键</param>
    /// <param name="prefix">要移除的前缀</param>
    /// <returns>移除前缀后的键</returns>
    public static string RemovePrefix(string fullKey, string prefix)
    {
        ArgumentNullException.ThrowIfNull(fullKey);
        ArgumentNullException.ThrowIfNull(prefix);

        var prefixWithColon = $"{prefix}:";
        return fullKey.StartsWith(prefixWithColon)
            ? fullKey[prefixWithColon.Length..]
            : fullKey;
    }

    /// <summary>
    /// 构建模式匹配字符串
    /// </summary>
    /// <param name="pattern">模式（使用 * 作为通配符）</param>
    /// <param name="prefix">前缀</param>
    /// <returns>完整的 Redis 模式</returns>
    public static string BuildPattern(string pattern, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        return string.IsNullOrEmpty(prefix)
            ? pattern
            : $"{prefix}:{pattern}";
    }
}
