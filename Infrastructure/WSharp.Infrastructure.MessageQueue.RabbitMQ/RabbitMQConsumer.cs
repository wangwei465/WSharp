using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WSharp.Infrastructure.MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ 消息消费者
/// </summary>
public class RabbitMQConsumer : MessageConsumerBase, IDisposable
{
    private readonly RabbitMQOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    /// <summary>
    /// 初始化 RabbitMQ 消费者
    /// </summary>
    public RabbitMQConsumer(
        IOptions<RabbitMQOptions> options,
        IServiceProvider serviceProvider,
        ILogger<RabbitMQConsumer> logger)
    {
        this._options = options.Value;
        this._serviceProvider = serviceProvider;
        this._logger = logger;
    }

    /// <summary>
    /// 开始消费消息
    /// </summary>
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 创建连接
            var factory = new ConnectionFactory
            {
                HostName = this._options.HostName,
                Port = this._options.Port,
                UserName = this._options.UserName,
                Password = this._options.Password,
                VirtualHost = this._options.VirtualHost,
                AutomaticRecoveryEnabled = this._options.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(this._options.NetworkRecoveryInterval),
                RequestedHeartbeat = TimeSpan.FromSeconds(this._options.RequestedHeartbeat),
                DispatchConsumersAsync = true
            };

            this._connection = factory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._channel.BasicQos(0, this._options.PrefetchCount, false);

            // 为每个订阅创建消费者
            foreach (var subscription in this.Subscriptions)
            {
                this.StartConsumer(subscription, cancellationToken);
            }

            this._logger.LogInformation("RabbitMQ consumer started with {Count} subscriptions", this.Subscriptions.Count);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error starting RabbitMQ consumer");
            throw;
        }
    }

    /// <summary>
    /// 停止消费消息
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            this._channel?.Close();
            this._connection?.Close();

            this._logger.LogInformation("RabbitMQ consumer stopped");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error stopping RabbitMQ consumer");
            throw;
        }
    }

    /// <summary>
    /// 启动消费者
    /// </summary>
    private void StartConsumer(SubscriptionInfo subscription, CancellationToken cancellationToken)
    {
        if (this._channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized");
        }

        // 声明队列
        this._channel.QueueDeclare(
            queue: subscription.QueueOrTopic,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // 创建异步消费者
        var consumer = new AsyncEventingBasicConsumer(this._channel);

        consumer.Received += async (model, ea) =>
        {
            await this.HandleMessageAsync(subscription, ea, cancellationToken);
        };

        // 开始消费
        this._channel.BasicConsume(
            queue: subscription.QueueOrTopic,
            autoAck: false,
            consumer: consumer);

        this._logger.LogInformation("Started consuming from queue {Queue}", subscription.QueueOrTopic);
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    private async Task HandleMessageAsync(
        SubscriptionInfo subscription,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        try
        {
            // 反序列化消息
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var message = JsonSerializer.Deserialize(json, subscription.MessageType) as IMessage;

            if (message == null)
            {
                this._logger.LogWarning("Failed to deserialize message from queue {Queue}", subscription.QueueOrTopic);
                this._channel?.BasicNack(eventArgs.DeliveryTag, false, false);
                return;
            }

            // 创建消息上下文
            var context = new MessageContext
            {
                MessageId = message.MessageId,
                CorrelationId = eventArgs.BasicProperties.CorrelationId,
                RetryCount = this.GetRetryCount(eventArgs),
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(eventArgs.BasicProperties.Timestamp.UnixTime).DateTime
            };

            // 处理消息
            using var scope = this._serviceProvider.CreateScope();
            var handlerType = typeof(IMessageHandler<>).MakeGenericType(subscription.MessageType);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");
            }

            var resultTask = handleMethod.Invoke(handler, new object[] { message, context, cancellationToken }) as Task<MessageHandlerResult>;
            if (resultTask == null)
            {
                throw new InvalidOperationException("HandleAsync returned null");
            }

            var result = await resultTask;

            if (result.Success)
            {
                // 确认消息
                this._channel?.BasicAck(eventArgs.DeliveryTag, false);
                this._logger.LogDebug("Message {MessageId} processed successfully", message.MessageId);
            }
            else if (result.ShouldRetry && context.RetryCount < 3)
            {
                // 拒绝并重新入队
                this._channel?.BasicNack(eventArgs.DeliveryTag, false, true);
                this._logger.LogWarning("Message {MessageId} processing failed, will retry. Error: {Error}",
                    message.MessageId, result.ErrorMessage);
            }
            else
            {
                // 拒绝并移到死信队列
                this._channel?.BasicNack(eventArgs.DeliveryTag, false, false);
                this._logger.LogError("Message {MessageId} processing failed after retries. Error: {Error}",
                    message.MessageId, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error handling message from queue {Queue}", subscription.QueueOrTopic);
            this._channel?.BasicNack(eventArgs.DeliveryTag, false, true);
        }
    }

    /// <summary>
    /// 获取重试次数
    /// </summary>
    private int GetRetryCount(BasicDeliverEventArgs eventArgs)
    {
        if (eventArgs.BasicProperties.Headers != null &&
            eventArgs.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryCountObj))
        {
            return Convert.ToInt32(retryCountObj);
        }

        return 0;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._channel?.Close();
        this._channel?.Dispose();
        this._connection?.Close();
        this._connection?.Dispose();

        this._disposed = true;
        GC.SuppressFinalize(this);
    }
}
