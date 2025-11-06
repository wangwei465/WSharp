using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;

namespace WSharp.Gateway.Transforms;

/// <summary>
/// 用于添加头部、身份认证等的自定义请求转换器
/// </summary>
public class CustomRequestTransform : RequestTransform
{
    private readonly ILogger<CustomRequestTransform> _logger;

    /// <summary>
    /// 初始化自定义请求转换器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public CustomRequestTransform(ILogger<CustomRequestTransform> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 应用请求转换
    /// </summary>
    /// <param name="context">请求转换上下文</param>
    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        // 添加关联 ID 头部
        if (!context.ProxyRequest.Headers.Contains("X-Correlation-ID"))
        {
            var correlationId = Guid.NewGuid().ToString();
            context.ProxyRequest.Headers.Add("X-Correlation-ID", correlationId);
            _logger.LogDebug("已添加关联 ID: {CorrelationId}", correlationId);
        }

        // 添加客户端 IP 头部
        var clientIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(clientIp))
        {
            context.ProxyRequest.Headers.Add("X-Forwarded-For", clientIp);
        }

        // 添加时间戳头部
        context.ProxyRequest.Headers.Add("X-Gateway-Timestamp", DateTimeOffset.UtcNow.ToString("o"));

        return default;
    }
}
