using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WSharp.Core.Application.Behaviors;

/// <summary>
/// 性能监控行为管道
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    /// <summary>
    /// 初始化性能监控行为
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        this._logger = logger;
        this._timer = new Stopwatch();
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
        this._timer.Start();

        var response = await next();

        this._timer.Stop();

        var elapsedMilliseconds = this._timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;

            this._logger.LogWarning(
                "长时间运行的请求: {RequestName} ({ElapsedMilliseconds} 毫秒) {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }

        return response;
    }
}
