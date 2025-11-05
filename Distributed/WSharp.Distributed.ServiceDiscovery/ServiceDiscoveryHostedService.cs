using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Background service for automatic service registration and deregistration
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
            _logger.LogInformation("Automatic service registration is disabled");
            return;
        }

        _logger.LogInformation("Starting service registration for: {ServiceName}", _options.ServiceName);

        var registered = await _serviceDiscovery.RegisterServiceAsync(cancellationToken);

        if (registered)
        {
            _logger.LogInformation("Service registration completed successfully");
        }
        else
        {
            _logger.LogWarning("Service registration failed");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.EnableAutoDeregistration)
        {
            _logger.LogInformation("Automatic service deregistration is disabled");
            return;
        }

        _logger.LogInformation("Starting service deregistration for: {ServiceName}", _options.ServiceName);

        var deregistered = await _serviceDiscovery.DeregisterServiceAsync(cancellationToken);

        if (deregistered)
        {
            _logger.LogInformation("Service deregistration completed successfully");
        }
        else
        {
            _logger.LogWarning("Service deregistration failed");
        }
    }
}
