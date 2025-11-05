# WSharp.Infrastructure.Caching.Redis

基于 StackExchange.Redis 的高性能 Redis 缓存实现，提供丰富的高级功能。

## 功能特性

- ✅ **完整的 ICacheService 实现** - 实现所有标准缓存操作
- ✅ **模式删除** - 使用 SCAN 命令安全删除匹配模式的键
- ✅ **批量操作** - 支持批量获取、设置、删除等高性能操作
- ✅ **发布/订阅** - 完整的 Redis Pub/Sub 支持，用于分布式缓存失效通知
- ✅ **Lua 脚本** - 内置常用 Lua 脚本，支持原子性操作
- ✅ **分布式锁** - 基于 Lua 脚本的可靠分布式锁实现
- ✅ **限流支持** - 滑动窗口限流算法
- ✅ **辅助工具** - 键构建器和序列化辅助类

## 安装

```bash
# 通过项目引用
<ProjectReference Include="..\WSharp.Infrastructure.Caching.Redis\WSharp.Infrastructure.Caching.Redis.csproj" />
```

## 快速开始

### 1. 基础用法

```csharp
using WSharp.Infrastructure.Caching.Redis;

// Program.cs 中配置
builder.Services.AddWSharpRedisCaching(
    connectionString: "localhost:6379",
    instanceName: "myapp",
    options =>
    {
        options.DefaultExpirationMinutes = 30;
        options.Database = 0;
        options.Password = "your-password"; // 可选
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

### 2. 带发布/订阅功能

```csharp
// 注册带 Pub/Sub 的服务
builder.Services.AddWSharpRedisCachingWithPubSub(
    connectionString: "localhost:6379",
    instanceName: "myapp",
    options =>
    {
        options.EnablePubSub = true;
    });

// 使用发布/订阅
public class NotificationService
{
    private readonly IRedisPubSubService _pubSubService;

    public NotificationService(IRedisPubSubService pubSubService)
    {
        _pubSubService = pubSubService;
    }

    // 发布消息
    public async Task PublishNotificationAsync(Notification notification)
    {
        await _pubSubService.PublishAsync("notifications", notification);
    }

    // 订阅消息
    public async Task SubscribeToNotificationsAsync()
    {
        await _pubSubService.SubscribeAsync<Notification>(
            "notifications",
            (channel, message) =>
            {
                Console.WriteLine($"Received: {message.Title}");
            });
    }
}
```

### 3. 完整功能版本

```csharp
// 注册所有高级功能
builder.Services.AddWSharpRedisCachingFull(
    connectionString: "localhost:6379",
    instanceName: "myapp",
    options =>
    {
        options.DefaultExpirationMinutes = 30;
        options.EnableBatchOperations = true;
        options.EnablePubSub = true;
        options.EnableScripting = true;
        options.ScanPageSize = 1000;
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

// 模式删除（使用 SCAN，安全高效）
await _cacheService.RemoveByPatternAsync("user:*");
```

### 批量操作

```csharp
// 批量获取
var keys = new[] { "key1", "key2", "key3" };
var results = await _redisCacheService.GetManyAsync<string>(keys);

// 批量设置
var items = new Dictionary<string, User>
{
    ["user:1"] = new User { Id = "1", Name = "Alice" },
    ["user:2"] = new User { Id = "2", Name = "Bob" }
};
await _redisCacheService.SetManyAsync(items, TimeSpan.FromMinutes(10));

// 批量删除
var deleteCount = await _redisCacheService.RemoveManyAsync(keys);

// 原子性递增/递减
var newValue = await _redisCacheService.IncrementAsync("counter", 1);
var newValue = await _redisCacheService.DecrementAsync("counter", 1);
```

### 发布/订阅

```csharp
// 发布消息
var subscriberCount = await _pubSubService.PublishAsync("channel", message);

// 订阅频道
await _pubSubService.SubscribeAsync<Message>(
    "channel",
    (ch, msg) => Console.WriteLine($"Got: {msg}"));

// 模式订阅（订阅匹配的所有频道）
await _pubSubService.SubscribePatternAsync<Message>(
    "cache:*",
    (ch, msg) => Console.WriteLine($"Channel {ch}: {msg}"));

// 取消订阅
await _pubSubService.UnsubscribeAsync("channel");

// 获取活跃频道
var channels = await _pubSubService.GetActiveChannelsAsync("cache:*");

// 获取订阅者数量
var count = await _pubSubService.GetSubscriberCountAsync("channel");
```

### Lua 脚本功能

```csharp
// 条件设置（SETNX）
bool success = await _scriptService.SetIfNotExistsAsync(
    "lock:resource",
    "unique-id",
    expirationSeconds: 30);

// 比较并设置（CAS）
bool updated = await _scriptService.CompareAndSetAsync(
    "config:version",
    expectedValue: "1.0",
    newValue: "1.1");

// 有界递增（不超过最大值）
long newValue = await _scriptService.BoundedIncrementAsync(
    "quota:user:123",
    incrementBy: 1,
    maxValue: 1000);

// 获取并删除
string value = await _scriptService.GetAndDeleteAsync("temp:token");

// 限流（滑动窗口）
bool allowed = await _scriptService.RateLimitAsync(
    "ratelimit:api:user:123",
    limit: 100,
    windowSeconds: 60);

// 分布式锁
var lockId = Guid.NewGuid().ToString();
bool acquired = await _scriptService.AcquireLockAsync(
    "lock:resource",
    lockId,
    expirationSeconds: 30);

if (acquired)
{
    try
    {
        // 执行需要锁保护的操作
    }
    finally
    {
        await _scriptService.ReleaseLockAsync("lock:resource", lockId);
    }
}

// 自定义 Lua 脚本
const string script = @"
    local value = redis.call('GET', KEYS[1])
    if value then
        return tonumber(value) * 2
    else
        return 0
    end
";
var result = await _scriptService.ExecuteScriptAsync(
    script,
    keys: new[] { (RedisKey)"mykey" });
```

### 辅助工具

#### 键构建器

```csharp
// 简单键构建
var key = RedisKeyBuilder.BuildKey("mykey", "prefix");
// 结果: "prefix:mykey"

// 多级键构建
var key = RedisKeyBuilder.BuildKey("app", "user", "123", "profile");
// 结果: "app:user:123:profile"

// 用户键
var key = RedisKeyBuilder.BuildUserKey("123", "settings", "myapp");
// 结果: "myapp:user:123:settings"

// 会话键
var key = RedisKeyBuilder.BuildSessionKey("abc123", "cart", "myapp");
// 结果: "myapp:session:abc123:cart"

// 锁键
var key = RedisKeyBuilder.BuildLockKey("resource:db:write", "myapp");
// 结果: "myapp:lock:resource:db:write"

// 限流键
var key = RedisKeyBuilder.BuildRateLimitKey("user:123", "api:login", "myapp");
// 结果: "myapp:ratelimit:api:login:user:123"

// 模式构建
var pattern = RedisKeyBuilder.BuildPattern("user:*", "myapp");
// 结果: "myapp:user:*"
```

#### 序列化辅助类

```csharp
// 序列化
var json = RedisSerializationHelper.Serialize(user);

// 反序列化
var user = RedisSerializationHelper.Deserialize<User>(json);

// 序列化为字节数组
var bytes = RedisSerializationHelper.SerializeToBytes(user);

// 从字节数组反序列化
var user = RedisSerializationHelper.DeserializeFromBytes<User>(bytes);

// 压缩序列化（适用于大对象）
var compressedBytes = RedisSerializationHelper.SerializeWithCompression(largeObject);
var obj = RedisSerializationHelper.DeserializeWithDecompression<MyType>(compressedBytes);

// 计算序列化大小
int sizeInBytes = RedisSerializationHelper.GetSerializedSize(user);

// 深度克隆对象
var clonedUser = RedisSerializationHelper.Clone(user);

// 安全序列化（不抛出异常）
if (RedisSerializationHelper.TrySerialize(user, out var json))
{
    // 序列化成功
}
```

## 配置选项

```csharp
public class RedisCacheOptions : CacheOptions
{
    // 基础配置
    public string ConnectionString { get; set; } = "localhost:6379";
    public string? InstanceName { get; set; }
    public int Database { get; set; } = 0;

    // 超时配置
    public int ConnectTimeout { get; set; } = 5000;      // 连接超时（毫秒）
    public int SyncTimeout { get; set; } = 5000;          // 同步操作超时
    public int AsyncTimeout { get; set; } = 5000;         // 异步操作超时

    // 安全配置
    public bool AllowAdmin { get; set; } = false;         // 是否允许管理员操作
    public bool UseSsl { get; set; } = false;             // 是否使用 SSL
    public string? Password { get; set; }                 // Redis 密码

    // 功能开关
    public bool EnableBatchOperations { get; set; } = true;
    public bool EnablePubSub { get; set; } = false;
    public bool EnableScripting { get; set; } = true;

    // 高级配置
    public int ScanPageSize { get; set; } = 1000;         // SCAN 分页大小
    public int RetryCount { get; set; } = 3;              // 重试次数
    public int RetryDelayMilliseconds { get; set; } = 100; // 重试延迟
}
```

## 最佳实践

### 1. 键命名规范

```csharp
// 推荐使用分层结构
"myapp:user:123:profile"
"myapp:session:abc:cart"
"myapp:cache:v1:product:456"

// 避免使用
"user123profile"
"UserProfile_123"
```

### 2. 过期时间策略

```csharp
// 热点数据：短过期
await _cacheService.SetAsync("trending:posts", posts, TimeSpan.FromMinutes(5));

// 常规数据：中等过期
await _cacheService.SetAsync("user:profile", user, TimeSpan.FromHours(1));

// 静态数据：长过期
await _cacheService.SetAsync("config:settings", settings, TimeSpan.FromDays(7));
```

### 3. 缓存穿透保护

```csharp
public async Task<User?> GetUserAsync(string userId)
{
    return await _cacheService.GetOrCreateAsync(
        $"user:{userId}",
        async () =>
        {
            var user = await _database.GetUserAsync(userId);
            // 即使为 null 也缓存，防止缓存穿透
            return user ?? User.Empty;
        },
        TimeSpan.FromMinutes(5)
    );
}
```

### 4. 分布式锁使用

```csharp
public async Task<bool> ProcessOrderAsync(string orderId)
{
    var lockKey = RedisKeyBuilder.BuildLockKey($"order:{orderId}");
    var lockId = Guid.NewGuid().ToString();

    // 尝试获取锁
    if (!await _scriptService.AcquireLockAsync(lockKey, lockId, 30))
    {
        return false; // 已被其他进程处理
    }

    try
    {
        // 执行订单处理逻辑
        await ProcessOrderLogicAsync(orderId);
        return true;
    }
    finally
    {
        // 确保释放锁
        await _scriptService.ReleaseLockAsync(lockKey, lockId);
    }
}
```

### 5. 限流实现

```csharp
public async Task<bool> CheckRateLimitAsync(string userId, string action)
{
    var rateLimitKey = RedisKeyBuilder.BuildRateLimitKey(userId, action);

    // 每分钟最多 100 次请求
    return await _scriptService.RateLimitAsync(
        rateLimitKey,
        limit: 100,
        windowSeconds: 60
    );
}
```

## 性能优化建议

1. **使用批量操作** - 减少网络往返次数
2. **合理设置过期时间** - 避免内存溢出
3. **使用 SCAN 而不是 KEYS** - 避免阻塞（已内置）
4. **压缩大对象** - 使用 `SerializeWithCompression`
5. **连接池复用** - `IConnectionMultiplexer` 是单例的

## 故障排查

### 连接超时

```csharp
builder.Services.AddWSharpRedisCaching(connectionString, instanceName, opt =>
{
    opt.ConnectTimeout = 10000;  // 增加超时时间
    opt.SyncTimeout = 10000;
    opt.AsyncTimeout = 10000;
});
```

### 大 Key 问题

```csharp
// 检查序列化大小
var size = RedisSerializationHelper.GetSerializedSize(largeObject);
if (size > 1024 * 1024) // 超过 1MB
{
    // 考虑压缩或分片存储
    var compressed = RedisSerializationHelper.SerializeWithCompression(largeObject);
}
```

## 相关项目

- **WSharp.Infrastructure.Caching** - 缓存抽象和基础实现
- **WSharp.Infrastructure.Caching.Memory** - 内存缓存实现

## License

MIT License
