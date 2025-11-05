using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WSharp.HealthChecks;

/// <summary>
/// Health check response writer
/// </summary>
public static class HealthCheckResponseWriter
{
    /// <summary>
    /// Write health check response as JSON
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
    /// Write simplified health check response
    /// </summary>
    public static async Task WriteSimpleResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "text/plain";

        var status = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
        await context.Response.WriteAsync(status);
    }
}
