using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息消费者接口
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// 订阅队列/主题
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="queueOrTopic">队列或主题名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SubscribeAsync<TMessage, THandler>(
        string queueOrTopic,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>;

    /// <summary>
    /// 开始消费消息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止消费消息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 消息消费者基类
/// </summary>
public abstract class MessageConsumerBase : IMessageConsumer
{
    /// <summary>
    /// 订阅信息
    /// </summary>
    protected readonly List<SubscriptionInfo> Subscriptions = new();

    /// <summary>
    /// 订阅队列/主题
    /// </summary>
    public virtual Task SubscribeAsync<TMessage, THandler>(
        string queueOrTopic,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>
    {
        this.Subscriptions.Add(new SubscriptionInfo
        {
            QueueOrTopic = queueOrTopic,
            MessageType = typeof(TMessage),
            HandlerType = typeof(THandler)
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 开始消费消息
    /// </summary>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止消费消息
    /// </summary>
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅信息
    /// </summary>
    protected class SubscriptionInfo
    {
        public string QueueOrTopic { get; set; } = string.Empty;
        public Type MessageType { get; set; } = null!;
        public Type HandlerType { get; set; } = null!;
    }
}

/// <summary>
/// 消息消费者后台服务
/// </summary>
public class MessageConsumerHostedService : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<MessageConsumerHostedService> _logger;

    /// <summary>
    /// 初始化消息消费者后台服务
    /// </summary>
    public MessageConsumerHostedService(
        IMessageConsumer consumer,
        ILogger<MessageConsumerHostedService> logger)
    {
        this._consumer = consumer;
        this._logger = logger;
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation("Message consumer hosted service is starting");

        try
        {
            await this._consumer.StartAsync(stoppingToken);

            // 保持运行直到取消
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            this._logger.LogInformation("Message consumer hosted service is stopping");
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error in message consumer hosted service");
            throw;
        }
    }

    /// <summary>
    /// 停止后台任务
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Message consumer hosted service is stopping");

        await this._consumer.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
