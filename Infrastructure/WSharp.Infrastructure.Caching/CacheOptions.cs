namespace WSharp.Infrastructure.Caching;

/// <summary>
/// 缓存配置选项
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// 默认过期时间（分钟）
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用压缩
    /// </summary>
    public bool EnableCompression { get; set; }

    /// <summary>
    /// 压缩阈值（字节）
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;
}

/// <summary>
/// 混合缓存配置选项
/// </summary>
public class HybridCacheOptions : CacheOptions
{
    /// <summary>
    /// L1 缓存（内存）过期时间（分钟）
    /// </summary>
    public int L1ExpirationMinutes { get; set; } = 5;

    /// <summary>
    /// L2 缓存（分布式）过期时间（分钟）
    /// </summary>
    public int L2ExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// 是否启用 L1 缓存
    /// </summary>
    public bool EnableL1Cache { get; set; } = true;

    /// <summary>
    /// 是否启用 L2 缓存
    /// </summary>
    public bool EnableL2Cache { get; set; } = true;
}
