using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;

namespace WSharp.Gateway.Transforms;

/// <summary>
/// Custom request transformer for adding headers, authentication, etc.
/// </summary>
public class CustomRequestTransform : RequestTransform
{
    private readonly ILogger<CustomRequestTransform> _logger;

    public CustomRequestTransform(ILogger<CustomRequestTransform> logger)
    {
        _logger = logger;
    }

    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        // Add correlation ID header
        if (!context.ProxyRequest.Headers.Contains("X-Correlation-ID"))
        {
            var correlationId = Guid.NewGuid().ToString();
            context.ProxyRequest.Headers.Add("X-Correlation-ID", correlationId);
            _logger.LogDebug("Added correlation ID: {CorrelationId}", correlationId);
        }

        // Add client IP header
        var clientIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(clientIp))
        {
            context.ProxyRequest.Headers.Add("X-Forwarded-For", clientIp);
        }

        // Add timestamp header
        context.ProxyRequest.Headers.Add("X-Gateway-Timestamp", DateTimeOffset.UtcNow.ToString("o"));

        return default;
    }
}
