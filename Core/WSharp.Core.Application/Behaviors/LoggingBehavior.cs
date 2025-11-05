using MediatR;
using Microsoft.Extensions.Logging;

namespace WSharp.Core.Application.Behaviors;

/// <summary>
/// 日志记录行为管道
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// 初始化日志记录行为
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="next">下一个处理器</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        this._logger.LogInformation(
            "处理请求: {RequestName} {@Request}",
            requestName,
            request);

        try
        {
            var response = await next();

            this._logger.LogInformation(
                "请求处理完成: {RequestName}",
                requestName);

            return response;
        }
        catch (Exception ex)
        {
            this._logger.LogError(
                ex,
                "请求处理失败: {RequestName} {@Request}",
                requestName,
                request);

            throw;
        }
    }
}
