using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Infrastructure.MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 RabbitMQ 消息队列
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRabbitMQ(
        this IServiceCollection services,
        Action<RabbitMQOptions> configureOptions)
    {
        // 配置选项
        services.Configure(configureOptions);

        // 注册生产者和消费者
        services.AddSingleton<IMessageProducer, RabbitMQProducer>();
        services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();

        // 注册后台服务
        services.AddHostedService<MessageConsumerHostedService>();

        return services;
    }

    /// <summary>
    /// 添加 RabbitMQ 生产者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRabbitMQProducer(
        this IServiceCollection services,
        Action<RabbitMQOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMessageProducer, RabbitMQProducer>();
        return services;
    }

    /// <summary>
    /// 添加 RabbitMQ 消费者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRabbitMQConsumer(
        this IServiceCollection services,
        Action<RabbitMQOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();
        services.AddHostedService<MessageConsumerHostedService>();
        return services;
    }
}
