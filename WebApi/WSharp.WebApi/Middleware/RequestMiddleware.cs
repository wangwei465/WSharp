using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WSharp.WebApi.Middleware;

/// <summary>
/// 请求日志中间件
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// 初始化中间件
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;
        var requestQuery = context.Request.QueryString;

        this._logger.LogInformation("Request: {Method} {Path}{Query}",
            requestMethod, requestPath, requestQuery);

        await this._next(context);

        stopwatch.Stop();

        this._logger.LogInformation("Response: {Method} {Path} - {StatusCode} in {ElapsedMilliseconds}ms",
            requestMethod, requestPath, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}

/// <summary>
/// 请求 ID 中间件
/// </summary>
public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequestIdHeader = "X-Request-ID";

    /// <summary>
    /// 初始化中间件
    /// </summary>
    public RequestIdMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers[RequestIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.TraceIdentifier = requestId;
        context.Response.Headers[RequestIdHeader] = requestId;

        await this._next(context);
    }
}

/// <summary>
/// CORS 中间件选项
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// 允许的源
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 允许的方法
    /// </summary>
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

    /// <summary>
    /// 允许的头
    /// </summary>
    public string[] AllowedHeaders { get; set; } = new[] { "*" };

    /// <summary>
    /// 是否允许凭证
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// 预检请求缓存时间（秒）
    /// </summary>
    public int MaxAge { get; set; } = 3600;
}
