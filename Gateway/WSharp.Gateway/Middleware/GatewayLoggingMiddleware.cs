using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WSharp.Gateway.Middleware;

/// <summary>
/// 用于请求/响应日志记录的中间件
/// </summary>
public class GatewayLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayLoggingMiddleware> _logger;

    /// <summary>
    /// 初始化网关日志记录中间件
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    public GatewayLoggingMiddleware(
        RequestDelegate next,
        ILogger<GatewayLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTimeOffset.UtcNow;
        context.Items["RequestStartTime"] = startTime;

        _logger.LogInformation(
            "网关请求: {Method} {Path} 来自 {ClientIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);

            var duration = DateTimeOffset.UtcNow - startTime;

            _logger.LogInformation(
                "网关响应: {Method} {Path} 返回 {StatusCode} 耗时 {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "网关错误: {Method} {Path} 失败，耗时 {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                (DateTimeOffset.UtcNow - startTime).TotalMilliseconds);
            throw;
        }
    }
}
