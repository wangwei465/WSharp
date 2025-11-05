using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Infrastructure.MessageQueue.Kafka;

/// <summary>
/// Kafka 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Kafka 消息队列
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        Action<KafkaOptions> configureOptions)
    {
        // 配置选项
        services.Configure(configureOptions);

        // 注册生产者和消费者
        services.AddSingleton<IMessageProducer, KafkaProducer>();
        services.AddSingleton<IMessageConsumer, KafkaConsumer>();

        // 注册后台服务
        services.AddHostedService<MessageConsumerHostedService>();

        return services;
    }

    /// <summary>
    /// 添加 Kafka 生产者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddKafkaProducer(
        this IServiceCollection services,
        Action<KafkaOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMessageProducer, KafkaProducer>();
        return services;
    }

    /// <summary>
    /// 添加 Kafka 消费者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services,
        Action<KafkaOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMessageConsumer, KafkaConsumer>();
        services.AddHostedService<MessageConsumerHostedService>();
        return services;
    }
}
