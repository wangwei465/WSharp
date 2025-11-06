using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace WSharp.HealthChecks;

/// <summary>
/// 磁盘空间健康检查
/// </summary>
public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly ILogger<DiskSpaceHealthCheck> _logger;
    private readonly string _driveName;
    private readonly long _minimumFreeMegabytes;

    public DiskSpaceHealthCheck(
        ILogger<DiskSpaceHealthCheck> logger,
        string driveName = "C:\\",
        long minimumFreeMegabytes = 1024)
    {
        _logger = logger;
        _driveName = driveName;
        _minimumFreeMegabytes = minimumFreeMegabytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var driveInfo = new DriveInfo(_driveName);

            if (!driveInfo.IsReady)
            {
                return Task.FromResult(new HealthCheckResult(
                    context.Registration.FailureStatus,
                    description: $"磁盘 {_driveName} 未就绪"));
            }

            var freeSpaceMB = driveInfo.AvailableFreeSpace / 1024 / 1024;
            var totalSpaceMB = driveInfo.TotalSize / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                { "Drive", _driveName },
                { "FreeSpaceMB", freeSpaceMB },
                { "TotalSpaceMB", totalSpaceMB },
                { "UsedSpaceMB", totalSpaceMB - freeSpaceMB }
            };

            _logger.LogDebug("磁盘健康检查 {Drive}: 剩余 {FreeSpaceMB}MB", _driveName, freeSpaceMB);

            var status = freeSpaceMB >= _minimumFreeMegabytes
                ? HealthStatus.Healthy
                : HealthStatus.Unhealthy;

            return Task.FromResult(new HealthCheckResult(
                status,
                description: $"磁盘 {_driveName}: {totalSpaceMB}MB 中剩余 {freeSpaceMB}MB",
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "磁盘健康检查失败，磁盘 {Drive}", _driveName);
            return Task.FromResult(new HealthCheckResult(
                context.Registration.FailureStatus,
                exception: ex));
        }
    }
}
