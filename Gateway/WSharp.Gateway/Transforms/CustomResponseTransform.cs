using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;

namespace WSharp.Gateway.Transforms;

/// <summary>
/// Custom response transformer for modifying responses
/// </summary>
public class CustomResponseTransform : ResponseTransform
{
    private readonly ILogger<CustomResponseTransform> _logger;

    public CustomResponseTransform(ILogger<CustomResponseTransform> logger)
    {
        _logger = logger;
    }

    public override ValueTask ApplyAsync(ResponseTransformContext context)
    {
        // Add gateway signature header
        context.HttpContext.Response.Headers.Add("X-Gateway", "WSharp");

        // Add processing time header
        if (context.HttpContext.Items.TryGetValue("RequestStartTime", out var startTimeObj)
            && startTimeObj is DateTimeOffset startTime)
        {
            var processingTime = DateTimeOffset.UtcNow - startTime;
            context.HttpContext.Response.Headers.Add("X-Processing-Time-Ms",
                processingTime.TotalMilliseconds.ToString("F2"));

            _logger.LogDebug("Request processed in {ProcessingTime}ms", processingTime.TotalMilliseconds);
        }

        // Remove sensitive headers
        context.HttpContext.Response.Headers.Remove("Server");
        context.HttpContext.Response.Headers.Remove("X-Powered-By");

        return default;
    }
}
