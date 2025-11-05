# WSharp.Infrastructure.Caching.Memory

高性能进程内内存缓存实现，提供丰富的缓存管理功能。

## 功能特性

- ✅ **完整的 ICacheService 实现** - 实现所有标准缓存操作
- ✅ **模式匹配删除** - 支持通配符模式删除缓存项
- ✅ **批量操作** - 支持批量获取、设置、删除等高性能操作
- ✅ **容量管理** - 支持大小和数量限制，自动内存压力管理
- ✅ **缓存统计** - 跟踪命中率、未命中率等统计信息
- ✅ **健康监控** - 实时监控缓存健康状态
- ✅ **灵活配置** - 支持滑动过期、绝对过期、优先级等多种配置

## 安装

```bash
# 通过项目引用
<ProjectReference Include="..\WSharp.Infrastructure.Caching.Memory\WSharp.Infrastructure.Caching.Memory.csproj" />
```

## 快速开始

### 1. 基础用法

```csharp
using WSharp.Infrastructure.Caching.Memory;

// Program.cs 中配置
builder.Services.AddWSharpMemoryCaching(options =>
{
    options.DefaultExpirationMinutes = 30;
    options.KeyPrefix = "myapp";
});

// 使用缓存服务
public class MyService
{
    private readonly ICacheService _cacheService;

    public MyService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<User> GetUserAsync(string userId)
    {
        var cacheKey = $"user:{userId}";

        // 获取或创建缓存
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () => await _database.GetUserAsync(userId),
            TimeSpan.FromMinutes(10)
        );
    }
}
```

### 2. 带统计功能

```csharp
// 启用统计功能
builder.Services.AddWSharpMemoryCachingWithStats(options =>
{
    options.EnableStatistics = true;
    options.DefaultExpirationMinutes = 30;
});

// 查看统计信息
public class CacheMonitorService
{
    private readonly MemoryCacheService _cacheService;

    public CacheMonitorService(ICacheService cacheService)
    {
        _cacheService = (MemoryCacheService)cacheService;
    }

    public void ShowStatistics()
    {
        var stats = _cacheService.GetStatistics();
        Console.WriteLine(stats.GetSummary());

        // 输出:
        // Memory Cache Statistics:
        // - Total Requests: 1,500
        // - Hit Count: 1,200 (80.00%)
        // - Miss Count: 300 (20.00%)
        // - Current Entries: 450
        // - Estimated Size: 45.00 KB
    }
}
```

### 3. 完整功能版本

```csharp
// 启用所有功能
builder.Services.AddWSharpMemoryCachingFull(options =>
{
    options.DefaultExpirationMinutes = 30;
    options.EnableStatistics = true;
    options.EnablePatternMatching = true;
    options.SizeLimit = 100 * 1024 * 1024; // 100 MB
    options.CountLimit = 10000; // 最多 10000 个条目
    options.CompactionPercentage = 0.05; // 压缩 5%
});
```

### 4. 带容量限制

```csharp
// 设置容量限制
builder.Services.AddWSharpMemoryCachingWithLimit(
    sizeLimit: 50 * 1024 * 1024,  // 50 MB
    countLimit: 5000,              // 5000 个条目
    options =>
    {
        options.DefaultExpirationMinutes = 15;
    });
```

## 核心功能详解

### 标准缓存操作

```csharp
// 设置缓存
await _cacheService.SetAsync("key", value, TimeSpan.FromMinutes(10));

// 获取缓存
var value = await _cacheService.GetAsync<MyType>("key");

// 删除缓存
await _cacheService.RemoveAsync("key");

// 检查是否存在
bool exists = await _cacheService.ExistsAsync("key");

// 刷新过期时间
await _cacheService.RefreshAsync("key");

// 模式删除（支持通配符）
await _cacheService.RemoveByPatternAsync("user:*");
```

### 批量操作

```csharp
// 批量获取
var keys = new[] { "key1", "key2", "key3" };
var results = await _memoryCacheService.GetManyAsync<string>(keys);

// 批量设置
var items = new Dictionary<string, User>
{
    ["user:1"] = new User { Id = "1", Name = "Alice" },
    ["user:2"] = new User { Id = "2", Name = "Bob" }
};
await _memoryCacheService.SetManyAsync(items, TimeSpan.FromMinutes(10));

// 批量删除
var deleteCount = await _memoryCacheService.RemoveManyAsync(keys);

// 批量检查存在
var existsResults = await _memoryCacheService.ExistsManyAsync(keys);

// 按前缀获取
var userCaches = await _memoryCacheService.GetByPrefixAsync<User>("user:");

// 按前缀删除
var removedCount = await _memoryCacheService.RemoveByPrefixAsync("temp:");
```

### 容量管理和统计

```csharp
// 获取统计信息
var stats = _memoryCacheService.GetStatistics();
Console.WriteLine($"Hit Rate: {stats.HitRate:F2}%");
Console.WriteLine($"Miss Rate: {stats.MissRate:F2}%");
Console.WriteLine($"Current Entries: {stats.CurrentEntryCount}");
Console.WriteLine($"Estimated Size: {stats.EstimatedSize} bytes");

// 获取健康状态
var health = _memoryCacheService.GetHealthStatus();
if (!health.IsHealthy)
{
    Console.WriteLine(health.GetSummary());
    foreach (var issue in health.Issues)
    {
        Console.WriteLine($"- {issue}");
    }
}

// 获取所有键
var allKeys = _memoryCacheService.GetAllKeys();

// 获取当前数量
int count = _memoryCacheService.GetCurrentCount();

// 检查是否达到容量限制
bool isAtLimit = _memoryCacheService.IsAtCapacityLimit();

// 手动压缩（释放 10% 的内存）
_memoryCacheService.Compact(0.10);

// 清空所有缓存
_memoryCacheService.Clear();

// 移除最旧的 100 个条目
int removed = _memoryCacheService.RemoveOldest(100);

// 重置统计计数器
_memoryCacheService.ResetStatistics();
```

### 模式匹配

```csharp
// 通配符删除（* 匹配任意字符，? 匹配单个字符）
await _cacheService.RemoveByPatternAsync("user:*");          // 删除所有 user: 开头的键
await _cacheService.RemoveByPatternAsync("session:???");     // 删除 session: 开头且后跟3个字符的键
await _cacheService.RemoveByPatternAsync("*:temp");          // 删除所有以 :temp 结尾的键
await _cacheService.RemoveByPatternAsync("cache:*:data");    // 删除匹配中间模式的键

// 按前缀批量操作
var userCaches = await _memoryCacheService.GetByPrefixAsync<User>("user:");
var removedCount = await _memoryCacheService.RemoveByPrefixAsync("temp:");
```

## 配置选项

```csharp
public class MemoryCacheOptions : CacheOptions
{
    // 基础配置（继承自 CacheOptions）
    public int DefaultExpirationMinutes { get; set; } = 30;
    public string KeyPrefix { get; set; } = string.Empty;

    // 容量限制
    public long? SizeLimit { get; set; }                    // 字节数限制
    public int? CountLimit { get; set; }                    // 条目数限制

    // 压缩和清理
    public double CompactionPercentage { get; set; } = 0.05; // 压缩百分比（0-1）
    public int ExpirationScanFrequencyMinutes { get; set; } = 1; // 过期扫描频率

    // 功能开关
    public bool EnableStatistics { get; set; } = false;      // 启用统计
    public bool EnablePatternMatching { get; set; } = true;  // 启用模式匹配
    public bool TrackSize { get; set; } = false;             // 跟踪大小

    // 过期策略
    public int? SlidingExpirationMinutes { get; set; }       // 滑动过期
    public int? AbsoluteExpirationMinutes { get; set; }      // 绝对过期

    // 优先级
    public CacheItemPriority DefaultPriority { get; set; } = CacheItemPriority.Normal;
}

// 优先级枚举
public enum CacheItemPriority
{
    Low = 0,         // 低优先级，首先被移除
    Normal = 1,      // 正常优先级
    High = 2,        // 高优先级，最后被移除
    NeverRemove = 3  // 永不移除（除非手动删除或过期）
}
```

## 最佳实践

### 1. 键命名规范

```csharp
// 推荐使用分层结构
"myapp:user:123:profile"
"myapp:session:abc:cart"
"myapp:cache:v1:product:456"

// 便于模式匹配删除
await _cacheService.RemoveByPatternAsync("myapp:user:*");
```

### 2. 过期时间策略

```csharp
// 热点数据：短过期
await _cacheService.SetAsync("trending:posts", posts, TimeSpan.FromMinutes(5));

// 常规数据：中等过期
await _cacheService.SetAsync("user:profile", user, TimeSpan.FromMinutes(30));

// 静态数据：长过期
await _cacheService.SetAsync("config:settings", settings, TimeSpan.FromHours(24));
```

### 3. 容量管理

```csharp
// 定期检查缓存健康状态
public class CacheHealthCheckService : BackgroundService
{
    private readonly MemoryCacheService _cacheService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var health = _cacheService.GetHealthStatus();

            if (!health.IsHealthy)
            {
                // 记录警告
                _logger.LogWarning("Cache health issues: {Issues}",
                    string.Join(", ", health.Issues));

                // 如果接近容量限制，执行压缩
                if (_cacheService.IsAtCapacityLimit())
                {
                    _cacheService.Compact(0.10); // 压缩 10%
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### 4. 缓存穿透保护

```csharp
public async Task<User?> GetUserAsync(string userId)
{
    return await _cacheService.GetOrCreateAsync(
        $"user:{userId}",
        async () =>
        {
            var user = await _database.GetUserAsync(userId);
            // 即使为 null 也缓存短时间，防止缓存穿透
            return user ?? User.Empty;
        },
        user == null ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(30)
    );
}
```

### 5. 批量操作优化

```csharp
// 推荐：使用批量操作
var userIds = new[] { "1", "2", "3", "4", "5" };
var keys = userIds.Select(id => $"user:{id}").ToArray();
var cachedUsers = await _memoryCacheService.GetManyAsync<User>(keys);

// 不推荐：循环调用单个操作
foreach (var userId in userIds)
{
    await _cacheService.GetAsync<User>($"user:{userId}"); // 性能较差
}
```

### 6. 统计监控

```csharp
// 定期输出统计信息
public void LogCacheStats()
{
    var stats = _memoryCacheService.GetStatistics();

    _logger.LogInformation(
        "Cache Stats - Requests: {Total}, Hit Rate: {HitRate:F2}%, " +
        "Entries: {Count}, Size: {Size:F2} MB",
        stats.TotalRequests,
        stats.HitRate,
        stats.CurrentEntryCount,
        stats.EstimatedSize / (1024.0 * 1024.0)
    );

    // 如果命中率过低，考虑调整缓存策略
    if (stats.TotalRequests > 1000 && stats.HitRate < 50)
    {
        _logger.LogWarning("Low cache hit rate: {HitRate:F2}%", stats.HitRate);
    }
}
```

## 性能优化建议

1. **合理设置容量限制** - 避免无限增长导致内存溢出
2. **使用批量操作** - 减少方法调用开销
3. **启用压缩策略** - 在内存压力下自动清理
4. **合理设置过期时间** - 平衡命中率和内存使用
5. **监控统计信息** - 及时发现性能问题

## 内存缓存 vs Redis 缓存

| 特性 | 内存缓存 | Redis 缓存 |
|------|---------|-----------|
| 性能 | 极快（进程内） | 快（网络通信） |
| 持久化 | 无 | 支持 |
| 分布式 | 否 | 是 |
| 容量 | 受限于进程内存 | 受限于 Redis 服务器 |
| 适用场景 | 单实例、临时数据 | 多实例、需要共享 |

## 常见问题

### 1. 模式匹配性能

```csharp
// 模式匹配需要遍历所有键，大规模场景下可能影响性能
// 建议：使用前缀匹配或限制缓存数量
await _cacheService.RemoveByPrefixAsync("user:");  // 比模式匹配快
```

### 2. 内存泄漏

```csharp
// 确保设置过期时间，避免缓存永久保留
await _cacheService.SetAsync("key", value, TimeSpan.FromMinutes(30)); // 正确
await _cacheService.SetAsync("key", value); // 使用默认过期时间
```

### 3. 统计功能开销

```csharp
// 统计功能有轻微性能开销，生产环境可选择性启用
builder.Services.AddWSharpMemoryCaching(options =>
{
    options.EnableStatistics = false; // 关闭统计以提升性能
});
```

## 相关项目

- **WSharp.Infrastructure.Caching** - 缓存抽象和基础实现
- **WSharp.Infrastructure.Caching.Redis** - Redis 缓存实现

## License

MIT License
