namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息处理器接口
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">消息上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理结果</returns>
    Task<MessageHandlerResult> HandleAsync(
        TMessage message,
        MessageContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 消息处理器基类
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
public abstract class MessageHandlerBase<TMessage> : IMessageHandler<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// 处理消息
    /// </summary>
    public async Task<MessageHandlerResult> HandleAsync(
        TMessage message,
        MessageContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await this.HandleMessageAsync(message, context, cancellationToken);
            return MessageHandlerResult.Successful();
        }
        catch (Exception ex)
        {
            return await this.HandleExceptionAsync(ex, message, context, cancellationToken);
        }
    }

    /// <summary>
    /// 处理消息的具体实现
    /// </summary>
    protected abstract Task HandleMessageAsync(
        TMessage message,
        MessageContext context,
        CancellationToken cancellationToken);

    /// <summary>
    /// 处理异常
    /// </summary>
    protected virtual Task<MessageHandlerResult> HandleExceptionAsync(
        Exception exception,
        TMessage message,
        MessageContext context,
        CancellationToken cancellationToken)
    {
        // 默认行为：记录错误，并根据重试次数决定是否重试
        var shouldRetry = context.RetryCount < 3;
        return Task.FromResult(MessageHandlerResult.Failed(exception.Message, shouldRetry));
    }
}
