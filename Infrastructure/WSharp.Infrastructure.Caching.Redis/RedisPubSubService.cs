using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 发布/订阅服务实现
/// </summary>
public class RedisPubSubService : IRedisPubSubService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ISubscriber _subscriber;
    private readonly RedisCacheOptions _options;

    public RedisPubSubService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisCacheOptions> options)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _subscriber = _connectionMultiplexer.GetSubscriber();
    }

    /// <summary>
    /// 发布消息到指定频道
    /// </summary>
    public async Task<long> PublishAsync<T>(string channel, T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channel);
        ArgumentNullException.ThrowIfNull(message);

        var fullChannel = BuildChannel(channel);
        var serializedMessage = Serialize(message);

        return await _subscriber.PublishAsync(fullChannel, serializedMessage);
    }

    /// <summary>
    /// 订阅指定频道
    /// </summary>
    public async Task SubscribeAsync<T>(string channel, Action<string, T> handler, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channel);
        ArgumentNullException.ThrowIfNull(handler);

        var fullChannel = BuildChannel(channel);

        await _subscriber.SubscribeAsync(fullChannel, (ch, value) =>
        {
            try
            {
                var message = Deserialize<T>(value!);
                if (message != null)
                {
                    handler(channel, message);
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不中断订阅
                Console.Error.WriteLine($"Error handling message on channel {channel}: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 订阅匹配模式的频道
    /// </summary>
    public async Task SubscribePatternAsync<T>(string pattern, Action<string, T> handler, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(handler);

        var fullPattern = BuildChannel(pattern);

        await _subscriber.SubscribeAsync(new RedisChannel(fullPattern, RedisChannel.PatternMode.Pattern), (ch, value) =>
        {
            try
            {
                var message = Deserialize<T>(value!);
                if (message != null)
                {
                    // 移除前缀返回原始频道名
                    var originalChannel = RemoveChannelPrefix(ch!);
                    handler(originalChannel, message);
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不中断订阅
                Console.Error.WriteLine($"Error handling message on pattern {pattern}: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 取消订阅指定频道
    /// </summary>
    public async Task UnsubscribeAsync(string channel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channel);

        var fullChannel = BuildChannel(channel);
        await _subscriber.UnsubscribeAsync(fullChannel);
    }

    /// <summary>
    /// 取消订阅所有频道
    /// </summary>
    public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        await _subscriber.UnsubscribeAllAsync();
    }

    /// <summary>
    /// 获取活跃的频道列表
    /// </summary>
    public async Task<IEnumerable<string>> GetActiveChannelsAsync(string? pattern = null, CancellationToken cancellationToken = default)
    {
        var server = GetServer();
        var fullPattern = string.IsNullOrEmpty(pattern) ? "*" : BuildChannel(pattern);

        var channels = await server.SubscriptionChannelsAsync(fullPattern);
        return channels.Select(ch => RemoveChannelPrefix(ch.ToString()));
    }

    /// <summary>
    /// 获取指定频道的订阅者数量
    /// </summary>
    public async Task<long> GetSubscriberCountAsync(string channel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channel);

        var server = GetServer();
        var fullChannel = BuildChannel(channel);

        var subscriptions = await server.SubscriptionSubscriberCountAsync(fullChannel);
        return subscriptions;
    }

    #region 辅助方法

    /// <summary>
    /// 构建完整的频道名称
    /// </summary>
    private string BuildChannel(string channel)
    {
        var prefix = !string.IsNullOrEmpty(_options.InstanceName)
            ? _options.InstanceName
            : _options.KeyPrefix;

        return string.IsNullOrEmpty(prefix)
            ? channel
            : $"{prefix}:{channel}";
    }

    /// <summary>
    /// 移除频道前缀
    /// </summary>
    private string RemoveChannelPrefix(string fullChannel)
    {
        var prefix = !string.IsNullOrEmpty(_options.InstanceName)
            ? _options.InstanceName
            : _options.KeyPrefix;

        if (string.IsNullOrEmpty(prefix))
        {
            return fullChannel;
        }

        var prefixWithColon = $"{prefix}:";
        return fullChannel.StartsWith(prefixWithColon)
            ? fullChannel[prefixWithColon.Length..]
            : fullChannel;
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
