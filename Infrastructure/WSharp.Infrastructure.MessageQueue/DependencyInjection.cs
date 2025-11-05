using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息队列依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加消息队列服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMessageQueue(
        this IServiceCollection services,
        Action<MessageQueueOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }

    /// <summary>
    /// 添加消息处理器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMessageHandler<TMessage, THandler>(this IServiceCollection services)
        where TMessage : IMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        services.TryAddScoped<IMessageHandler<TMessage>, THandler>();
        return services;
    }

    /// <summary>
    /// 添加消息生产者
    /// </summary>
    /// <typeparam name="TProducer">生产者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMessageProducer<TProducer>(this IServiceCollection services)
        where TProducer : class, IMessageProducer
    {
        services.TryAddSingleton<IMessageProducer, TProducer>();
        return services;
    }

    /// <summary>
    /// 添加消息消费者
    /// </summary>
    /// <typeparam name="TConsumer">消费者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMessageConsumer<TConsumer>(this IServiceCollection services)
        where TConsumer : class, IMessageConsumer
    {
        services.TryAddSingleton<IMessageConsumer, TConsumer>();
        services.AddHostedService<MessageConsumerHostedService>();
        return services;
    }

    /// <summary>
    /// 配置消息消费订阅
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="queueOrTopic">队列或主题名称</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMessageSubscription<TMessage, THandler>(
        this IServiceCollection services,
        string queueOrTopic)
        where TMessage : IMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        // 注册处理器
        services.AddMessageHandler<TMessage, THandler>();

        // 存储订阅信息（实际的订阅将在消费者启动时执行）
        services.Configure<MessageSubscriptionOptions>(options =>
        {
            options.Subscriptions.Add(new MessageSubscription
            {
                QueueOrTopic = queueOrTopic,
                MessageType = typeof(TMessage),
                HandlerType = typeof(THandler)
            });
        });

        return services;
    }
}

/// <summary>
/// 消息订阅选项
/// </summary>
public class MessageSubscriptionOptions
{
    /// <summary>
    /// 订阅列表
    /// </summary>
    public List<MessageSubscription> Subscriptions { get; set; } = new();
}

/// <summary>
/// 消息订阅
/// </summary>
public class MessageSubscription
{
    /// <summary>
    /// 队列或主题名称
    /// </summary>
    public string QueueOrTopic { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型
    /// </summary>
    public Type MessageType { get; set; } = null!;

    /// <summary>
    /// 处理器类型
    /// </summary>
    public Type HandlerType { get; set; } = null!;
}
