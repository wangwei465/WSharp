using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WSharp.HealthChecks;

/// <summary>
/// 健康检查响应写入器
/// </summary>
public static class HealthCheckResponseWriter
{
    /// <summary>
    /// 将健康检查响应写入为 JSON 格式
    /// </summary>
    public static async Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var result = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                exception = e.Value.Exception?.Message,
                data = e.Value.Data
            })
        };

        var json = JsonSerializer.Serialize(result, options);
        await context.Response.WriteAsync(json, Encoding.UTF8);
    }

    /// <summary>
    /// 写入简化的健康检查响应
    /// </summary>
    public static async Task WriteSimpleResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "text/plain";

        var status = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
        await context.Response.WriteAsync(status);
    }
}
