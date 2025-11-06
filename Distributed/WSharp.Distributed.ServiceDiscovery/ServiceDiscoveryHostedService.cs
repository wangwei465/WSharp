using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// 用于自动服务注册和注销的后台服务
/// </summary>
public class ServiceDiscoveryHostedService : IHostedService
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ServiceDiscoveryOptions _options;
    private readonly ILogger<ServiceDiscoveryHostedService> _logger;

    public ServiceDiscoveryHostedService(
        IServiceDiscovery serviceDiscovery,
        IOptions<ServiceDiscoveryOptions> options,
        ILogger<ServiceDiscoveryHostedService> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.EnableAutoRegistration)
        {
            _logger.LogInformation("自动服务注册已禁用");
            return;
        }

        _logger.LogInformation("开始服务注册: {ServiceName}", _options.ServiceName);

        var registered = await _serviceDiscovery.RegisterServiceAsync(cancellationToken);

        if (registered)
        {
            _logger.LogInformation("服务注册成功完成");
        }
        else
        {
            _logger.LogWarning("服务注册失败");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.EnableAutoDeregistration)
        {
            _logger.LogInformation("自动服务注销已禁用");
            return;
        }

        _logger.LogInformation("开始服务注销: {ServiceName}", _options.ServiceName);

        var deregistered = await _serviceDiscovery.DeregisterServiceAsync(cancellationToken);

        if (deregistered)
        {
            _logger.LogInformation("服务注销成功完成");
        }
        else
        {
            _logger.LogWarning("服务注销失败");
        }
    }
}
