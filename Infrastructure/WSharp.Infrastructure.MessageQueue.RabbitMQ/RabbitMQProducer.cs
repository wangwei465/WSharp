using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace WSharp.Infrastructure.MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ 消息生产者
/// </summary>
public class RabbitMQProducer : MessageProducerBase, IDisposable
{
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQProducer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private bool _disposed;

    /// <summary>
    /// 初始化 RabbitMQ 生产者
    /// </summary>
    public RabbitMQProducer(
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQProducer> logger)
    {
        this._options = options.Value;
        this._logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = this._options.HostName,
            Port = this._options.Port,
            UserName = this._options.UserName,
            Password = this._options.Password,
            VirtualHost = this._options.VirtualHost,
            AutomaticRecoveryEnabled = this._options.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(this._options.NetworkRecoveryInterval),
            RequestedHeartbeat = TimeSpan.FromSeconds(this._options.RequestedHeartbeat)
        };

        this._connection = factory.CreateConnection();
        this._channel = this._connection.CreateModel();
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    public override Task PublishAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        MessageContext? context = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 声明交换机和队列
            this.DeclareExchangeAndQueue(queueOrTopic);

            // 序列化消息
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // 设置消息属性
            var properties = this._channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = message.MessageId;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            if (context != null)
            {
                properties.CorrelationId = context.CorrelationId ?? string.Empty;
                properties.Headers = new Dictionary<string, object>();
                foreach (var prop in context.Properties)
                {
                    properties.Headers[prop.Key] = prop.Value;
                }
            }

            // 发布消息
            var exchange = this._options.DefaultExchange;
            this._channel.BasicPublish(
                exchange: exchange,
                routingKey: queueOrTopic,
                basicProperties: properties,
                body: body);

            this._logger.LogDebug("Published message {MessageId} to {Queue}", message.MessageId, queueOrTopic);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error publishing message to {Queue}", queueOrTopic);
            throw;
        }
    }

    /// <summary>
    /// 发布延迟消息
    /// </summary>
    public override Task PublishDelayedAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        TimeSpan delay,
        MessageContext? context = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 声明延迟队列和死信交换机
            var delayedQueue = $"{queueOrTopic}.delayed";
            this.DeclareDelayedQueue(queueOrTopic, delayedQueue, delay);

            // 序列化消息
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // 设置消息属性
            var properties = this._channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = message.MessageId;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Expiration = ((int)delay.TotalMilliseconds).ToString();

            if (context != null)
            {
                properties.CorrelationId = context.CorrelationId ?? string.Empty;
                properties.Headers = new Dictionary<string, object>();
                foreach (var prop in context.Properties)
                {
                    properties.Headers[prop.Key] = prop.Value;
                }
            }

            // 发布到延迟队列
            this._channel.BasicPublish(
                exchange: string.Empty,
                routingKey: delayedQueue,
                basicProperties: properties,
                body: body);

            this._logger.LogDebug("Published delayed message {MessageId} to {Queue} with delay {Delay}",
                message.MessageId, queueOrTopic, delay);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error publishing delayed message to {Queue}", queueOrTopic);
            throw;
        }
    }

    /// <summary>
    /// 声明交换机和队列
    /// </summary>
    private void DeclareExchangeAndQueue(string queueName)
    {
        // 声明交换机
        if (!string.IsNullOrEmpty(this._options.DefaultExchange))
        {
            this._channel.ExchangeDeclare(
                exchange: this._options.DefaultExchange,
                type: this._options.ExchangeType,
                durable: true,
                autoDelete: false);
        }

        // 声明队列
        this._channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // 绑定队列到交换机
        if (!string.IsNullOrEmpty(this._options.DefaultExchange))
        {
            this._channel.QueueBind(
                queue: queueName,
                exchange: this._options.DefaultExchange,
                routingKey: queueName);
        }
    }

    /// <summary>
    /// 声明延迟队列
    /// </summary>
    private void DeclareDelayedQueue(string targetQueue, string delayedQueue, TimeSpan delay)
    {
        // 声明目标队列
        this.DeclareExchangeAndQueue(targetQueue);

        // 声明延迟队列，配置 DLX 指向目标队列
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", this._options.DefaultExchange },
            { "x-dead-letter-routing-key", targetQueue },
            { "x-message-ttl", (int)delay.TotalMilliseconds }
        };

        this._channel.QueueDeclare(
            queue: delayedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);
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
