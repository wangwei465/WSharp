using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSharp.Infrastructure.MessageQueue.Kafka;

/// <summary>
/// Kafka 消息生产者
/// </summary>
public class KafkaProducer : MessageProducerBase, IDisposable
{
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    /// <summary>
    /// 初始化 Kafka 生产者
    /// </summary>
    public KafkaProducer(
        IOptions<KafkaOptions> options,
        ILogger<KafkaProducer> logger)
    {
        this._options = options.Value;
        this._logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = this._options.BootstrapServers,
            Acks = Enum.Parse<Acks>(this._options.Acks, true),
            MessageSendMaxRetries = this._options.Retries,
            BatchSize = this._options.BatchSize,
            LingerMs = this._options.LingerMs,
            CompressionType = Enum.Parse<CompressionType>(this._options.CompressionType, true),
            SecurityProtocol = Enum.Parse<SecurityProtocol>(this._options.SecurityProtocol, true)
        };

        if (!string.IsNullOrEmpty(this._options.SaslMechanism))
        {
            config.SaslMechanism = Enum.Parse<SaslMechanism>(this._options.SaslMechanism, true);
            config.SaslUsername = this._options.SaslUsername;
            config.SaslPassword = this._options.SaslPassword;
        }

        this._producer = new ProducerBuilder<string, string>(config).Build();
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    public override async Task PublishAsync<TMessage>(
        string queueOrTopic,
        TMessage message,
        MessageContext? context = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.MessageId,
                Value = json,
                Headers = new Headers()
            };

            if (context != null)
            {
                if (!string.IsNullOrEmpty(context.CorrelationId))
                {
                    kafkaMessage.Headers.Add("CorrelationId", System.Text.Encoding.UTF8.GetBytes(context.CorrelationId));
                }

                foreach (var prop in context.Properties)
                {
                    var propValue = prop.Value.ToString() ?? string.Empty;
                    kafkaMessage.Headers.Add(prop.Key, System.Text.Encoding.UTF8.GetBytes(propValue));
                }
            }

            var result = await this._producer.ProduceAsync(queueOrTopic, kafkaMessage, cancellationToken);

            this._logger.LogDebug("Published message {MessageId} to topic {Topic} at offset {Offset}",
                message.MessageId, queueOrTopic, result.Offset);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error publishing message to topic {Topic}", queueOrTopic);
            throw;
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

        this._producer?.Flush(TimeSpan.FromSeconds(10));
        this._producer?.Dispose();

        this._disposed = true;
        GC.SuppressFinalize(this);
    }
}
