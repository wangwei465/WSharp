using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSharp.Infrastructure.MessageQueue.Kafka;

/// <summary>
/// Kafka 消息消费者
/// </summary>
public class KafkaConsumer : MessageConsumerBase, IDisposable
{
    private readonly KafkaOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, string>? _consumer;
    private bool _disposed;
    private CancellationTokenSource? _cts;

    /// <summary>
    /// 初始化 Kafka 消费者
    /// </summary>
    public KafkaConsumer(
        IOptions<KafkaOptions> options,
        IServiceProvider serviceProvider,
        ILogger<KafkaConsumer> logger)
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
            var config = new ConsumerConfig
            {
                BootstrapServers = this._options.BootstrapServers,
                GroupId = this._options.GroupId,
                EnableAutoCommit = this._options.EnableAutoCommit,
                AutoCommitIntervalMs = this._options.AutoCommitIntervalMs,
                SessionTimeoutMs = this._options.SessionTimeoutMs,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(this._options.AutoOffsetReset, true),
                SecurityProtocol = Enum.Parse<SecurityProtocol>(this._options.SecurityProtocol, true)
            };

            if (!string.IsNullOrEmpty(this._options.SaslMechanism))
            {
                config.SaslMechanism = Enum.Parse<SaslMechanism>(this._options.SaslMechanism, true);
                config.SaslUsername = this._options.SaslUsername;
                config.SaslPassword = this._options.SaslPassword;
            }

            this._consumer = new ConsumerBuilder<string, string>(config).Build();

            // 订阅所有主题
            var topics = this.Subscriptions.Select(s => s.QueueOrTopic).Distinct().ToList();
            this._consumer.Subscribe(topics);

            this._cts = new CancellationTokenSource();
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._cts.Token).Token;

            // 启动消费循环
            Task.Run(() => this.ConsumeLoop(combinedToken), combinedToken);

            this._logger.LogInformation("Kafka consumer started with {Count} subscriptions", this.Subscriptions.Count);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error starting Kafka consumer");
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
            this._cts?.Cancel();
            this._consumer?.Close();

            this._logger.LogInformation("Kafka consumer stopped");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error stopping Kafka consumer");
            throw;
        }
    }

    /// <summary>
    /// 消费循环
    /// </summary>
    private async Task ConsumeLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && this._consumer != null)
        {
            try
            {
                var consumeResult = this._consumer.Consume(cancellationToken);

                if (consumeResult != null)
                {
                    await this.HandleMessageAsync(consumeResult, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in consume loop");
            }
        }
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    private async Task HandleMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        var topic = consumeResult.Topic;
        var subscription = this.Subscriptions.FirstOrDefault(s => s.QueueOrTopic == topic);

        if (subscription == null)
        {
            this._logger.LogWarning("No subscription found for topic {Topic}", topic);
            return;
        }

        try
        {
            // 反序列化消息
            var message = JsonSerializer.Deserialize(consumeResult.Message.Value, subscription.MessageType) as IMessage;

            if (message == null)
            {
                this._logger.LogWarning("Failed to deserialize message from topic {Topic}", topic);
                return;
            }

            // 创建消息上下文
            var context = new MessageContext
            {
                MessageId = message.MessageId,
                Timestamp = consumeResult.Message.Timestamp.UtcDateTime
            };

            // 从消息头读取 CorrelationId
            if (consumeResult.Message.Headers != null)
            {
                var correlationIdHeader = consumeResult.Message.Headers.FirstOrDefault(h => h.Key == "CorrelationId");
                if (correlationIdHeader != null)
                {
                    context.CorrelationId = System.Text.Encoding.UTF8.GetString(correlationIdHeader.GetValueBytes());
                }
            }

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
                // 手动提交偏移量
                if (!this._options.EnableAutoCommit)
                {
                    this._consumer?.Commit(consumeResult);
                }

                this._logger.LogDebug("Message {MessageId} processed successfully", message.MessageId);
            }
            else
            {
                this._logger.LogError("Message {MessageId} processing failed. Error: {Error}",
                    message.MessageId, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error handling message from topic {Topic}", topic);
        }
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

        this._cts?.Cancel();
        this._cts?.Dispose();
        this._consumer?.Close();
        this._consumer?.Dispose();

        this._disposed = true;
        GC.SuppressFinalize(this);
    }
}
