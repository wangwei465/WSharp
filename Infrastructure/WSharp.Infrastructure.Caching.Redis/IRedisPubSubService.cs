namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 发布/订阅服务接口
/// </summary>
public interface IRedisPubSubService
{
    /// <summary>
    /// 发布消息到指定频道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">频道名称</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>接收到消息的订阅者数量</returns>
    Task<long> PublishAsync<T>(string channel, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅指定频道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">频道名称</param>
    /// <param name="handler">消息处理器</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SubscribeAsync<T>(string channel, Action<string, T> handler, CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅匹配模式的频道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="pattern">频道模式（如 "cache:*"）</param>
    /// <param name="handler">消息处理器</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SubscribePatternAsync<T>(string pattern, Action<string, T> handler, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅指定频道
    /// </summary>
    /// <param name="channel">频道名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UnsubscribeAsync(string channel, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅所有频道
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task UnsubscribeAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的频道列表
    /// </summary>
    /// <param name="pattern">频道模式（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>频道列表</returns>
    Task<IEnumerable<string>> GetActiveChannelsAsync(string? pattern = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定频道的订阅者数量
    /// </summary>
    /// <param name="channel">频道名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>订阅者数量</returns>
    Task<long> GetSubscriberCountAsync(string channel, CancellationToken cancellationToken = default);
}
