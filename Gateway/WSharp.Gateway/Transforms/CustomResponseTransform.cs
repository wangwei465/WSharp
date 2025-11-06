using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;

namespace WSharp.Gateway.Transforms;

/// <summary>
/// 用于修改响应的自定义响应转换器
/// </summary>
public class CustomResponseTransform : ResponseTransform
{
    private readonly ILogger<CustomResponseTransform> _logger;

    /// <summary>
    /// 初始化自定义响应转换器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public CustomResponseTransform(ILogger<CustomResponseTransform> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 应用响应转换
    /// </summary>
    /// <param name="context">响应转换上下文</param>
    public override ValueTask ApplyAsync(ResponseTransformContext context)
    {
        // 添加网关签名头部
        context.HttpContext.Response.Headers.Add("X-Gateway", "WSharp");

        // 添加处理时间头部
        if (context.HttpContext.Items.TryGetValue("RequestStartTime", out var startTimeObj)
            && startTimeObj is DateTimeOffset startTime)
        {
            var processingTime = DateTimeOffset.UtcNow - startTime;
            context.HttpContext.Response.Headers.Add("X-Processing-Time-Ms",
                processingTime.TotalMilliseconds.ToString("F2"));

            _logger.LogDebug("请求处理耗时 {ProcessingTime}ms", processingTime.TotalMilliseconds);
        }

        // 移除敏感头部
        context.HttpContext.Response.Headers.Remove("Server");
        context.HttpContext.Response.Headers.Remove("X-Powered-By");

        return default;
    }
}
