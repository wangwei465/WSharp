namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息生产者接口
/// </summary>
public interface IMessageProducer
{
    /// <summary>
    /// 发布消息到指定队列/主题
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="queueOrTopic">队列或主题名称</param>
    /// <param name="message">消息</param>
    /// <param name="context">消息上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        MessageContext? context = null,
        CancellationToken cancellationToken = default) where TMessage : IMessage;

    /// <summary>
    /// 批量发布消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="queueOrTopic">队列或主题名称</param>
    /// <param name="messages">消息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishBatchAsync<TMessage>(
        string queueOrTopic,
        IEnumerable<TMessage> messages,
        CancellationToken cancellationToken = default) where TMessage : IMessage;

    /// <summary>
    /// 发布延迟消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="queueOrTopic">队列或主题名称</param>
    /// <param name="message">消息</param>
    /// <param name="delay">延迟时间</param>
    /// <param name="context">消息上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishDelayedAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        TimeSpan delay,
        MessageContext? context = null,
        CancellationToken cancellationToken = default) where TMessage : IMessage;
}

/// <summary>
/// 消息生产者基类
/// </summary>
public abstract class MessageProducerBase : IMessageProducer
{
    /// <summary>
    /// 发布消息
    /// </summary>
    public abstract Task PublishAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        MessageContext? context = null,
        CancellationToken cancellationToken = default) where TMessage : IMessage;

    /// <summary>
    /// 批量发布消息
    /// </summary>
    public virtual async Task PublishBatchAsync<TMessage>(
        string queueOrTopic,
        IEnumerable<TMessage> messages,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        foreach (var message in messages)
        {
            await this.PublishAsync(queueOrTopic, message, null, cancellationToken);
        }
    }

    /// <summary>
    /// 发布延迟消息（默认实现不支持，子类可以重写）
    /// </summary>
    public virtual Task PublishDelayedAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        TimeSpan delay,
        MessageContext? context = null,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        throw new NotSupportedException("Delayed messages are not supported by this provider");
    }
}
