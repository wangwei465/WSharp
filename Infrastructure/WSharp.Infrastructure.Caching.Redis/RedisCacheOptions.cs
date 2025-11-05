namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 缓存配置选项
/// </summary>
public class RedisCacheOptions : CacheOptions
{
    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Redis 实例名称（用作键前缀）
    /// </summary>
    public string? InstanceName { get; set; }

    /// <summary>
    /// 数据库索引（0-15，默认为 0）
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// 连接超时时间（毫秒）
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// 同步操作超时时间（毫秒）
    /// </summary>
    public int SyncTimeout { get; set; } = 5000;

    /// <summary>
    /// 异步操作超时时间（毫秒）
    /// </summary>
    public int AsyncTimeout { get; set; } = 5000;

    /// <summary>
    /// 是否允许管理员操作（如 FLUSHDB）
    /// </summary>
    public bool AllowAdmin { get; set; } = false;

    /// <summary>
    /// 是否启用 SSL/TLS 加密连接
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Redis 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 是否启用批量操作功能
    /// </summary>
    public bool EnableBatchOperations { get; set; } = true;

    /// <summary>
    /// 是否启用发布/订阅功能
    /// </summary>
    public bool EnablePubSub { get; set; } = false;

    /// <summary>
    /// 是否启用 Lua 脚本功能
    /// </summary>
    public bool EnableScripting { get; set; } = true;

    /// <summary>
    /// 模式扫描时每次获取的键数量（用于 RemoveByPatternAsync）
    /// </summary>
    public int ScanPageSize { get; set; } = 1000;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// 重试延迟（毫秒）
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 100;
}
