using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace WSharp.HealthChecks;

/// <summary>
/// 内存使用健康检查
/// </summary>
public class MemoryHealthCheck : IHealthCheck
{
    private readonly ILogger<MemoryHealthCheck> _logger;
    private readonly long _thresholdBytes;

    public MemoryHealthCheck(ILogger<MemoryHealthCheck> logger, long thresholdBytes = 1024 * 1024 * 1024)
    {
        _logger = logger;
        _thresholdBytes = thresholdBytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>
            {
                { "AllocatedBytes", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            _logger.LogDebug("内存健康检查: 已分配 {AllocatedBytes} 字节", allocated);

            var status = allocated < _thresholdBytes
                ? HealthStatus.Healthy
                : HealthStatus.Degraded;

            return Task.FromResult(new HealthCheckResult(
                status,
                description: $"内存使用: {allocated / 1024 / 1024}MB",
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "内存健康检查失败");
            return Task.FromResult(new HealthCheckResult(
                context.Registration.FailureStatus,
                exception: ex));
        }
    }
}
