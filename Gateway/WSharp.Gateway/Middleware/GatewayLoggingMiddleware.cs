using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WSharp.Gateway.Middleware;

/// <summary>
/// Middleware for request/response logging
/// </summary>
public class GatewayLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayLoggingMiddleware> _logger;

    public GatewayLoggingMiddleware(
        RequestDelegate next,
        ILogger<GatewayLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTimeOffset.UtcNow;
        context.Items["RequestStartTime"] = startTime;

        _logger.LogInformation(
            "Gateway request: {Method} {Path} from {ClientIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);

            var duration = DateTimeOffset.UtcNow - startTime;

            _logger.LogInformation(
                "Gateway response: {Method} {Path} returned {StatusCode} in {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Gateway error: {Method} {Path} failed after {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                (DateTimeOffset.UtcNow - startTime).TotalMilliseconds);
            throw;
        }
    }
}
