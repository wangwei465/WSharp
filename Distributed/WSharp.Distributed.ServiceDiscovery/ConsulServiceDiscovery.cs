using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Consul-based service discovery implementation
/// </summary>
public class ConsulServiceDiscovery : IServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    private readonly ServiceDiscoveryOptions _options;
    private readonly ILogger<ConsulServiceDiscovery> _logger;
    private readonly Random _random = new();

    public ConsulServiceDiscovery(
        IConsulClient consulClient,
        IOptions<ServiceDiscoveryOptions> options,
        ILogger<ConsulServiceDiscovery> logger)
    {
        _consulClient = consulClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> RegisterServiceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = new AgentServiceRegistration
            {
                ID = _options.ServiceId,
                Name = _options.ServiceName,
                Address = _options.ServiceAddress,
                Port = _options.ServicePort,
                Tags = _options.Tags,
                Meta = _options.Metadata
            };

            // Configure health check if endpoint is provided
            if (!string.IsNullOrEmpty(_options.HealthCheckEndpoint))
            {
                var healthCheckUrl = $"http://{_options.ServiceAddress}:{_options.ServicePort}{_options.HealthCheckEndpoint}";
                registration.Check = new AgentServiceCheck
                {
                    HTTP = healthCheckUrl,
                    Interval = TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds),
                    Timeout = TimeSpan.FromSeconds(_options.HealthCheckTimeoutSeconds),
                    DeregisterCriticalServiceAfter = TimeSpan.Parse(_options.DeregisterCriticalServiceAfter)
                };
            }

            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
            _logger.LogInformation(
                "Service registered successfully: {ServiceName} ({ServiceId}) at {Address}:{Port}",
                _options.ServiceName, _options.ServiceId, _options.ServiceAddress, _options.ServicePort);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register service: {ServiceName} ({ServiceId})",
                _options.ServiceName, _options.ServiceId);
            return false;
        }
    }

    public async Task<bool> DeregisterServiceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _consulClient.Agent.ServiceDeregister(_options.ServiceId, cancellationToken);
            _logger.LogInformation(
                "Service deregistered successfully: {ServiceName} ({ServiceId})",
                _options.ServiceName, _options.ServiceId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deregister service: {ServiceName} ({ServiceId})",
                _options.ServiceName, _options.ServiceId);
            return false;
        }
    }

    public async Task<IEnumerable<ServiceRegistration>> GetHealthyServicesAsync(
        string serviceName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var services = await _consulClient.Health.Service(serviceName, null, true, cancellationToken);
            return services.Response.Select(MapToServiceRegistration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get healthy services for: {ServiceName}", serviceName);
            return Enumerable.Empty<ServiceRegistration>();
        }
    }

    public async Task<IEnumerable<ServiceRegistration>> GetAllServicesAsync(
        string serviceName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var services = await _consulClient.Health.Service(serviceName, null, false, cancellationToken);
            return services.Response.Select(MapToServiceRegistration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all services for: {ServiceName}", serviceName);
            return Enumerable.Empty<ServiceRegistration>();
        }
    }

    public async Task<ServiceRegistration?> GetServiceAsync(
        string serviceName,
        CancellationToken cancellationToken = default)
    {
        var services = (await GetHealthyServicesAsync(serviceName, cancellationToken)).ToList();

        if (!services.Any())
        {
            _logger.LogWarning("No healthy services found for: {ServiceName}", serviceName);
            return null;
        }

        // Simple random load balancing
        var index = _random.Next(services.Count);
        return services[index];
    }

    public async Task<IEnumerable<string>> GetAllServiceNamesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var services = await _consulClient.Catalog.Services(cancellationToken);
            return services.Response.Keys;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all service names");
            return Enumerable.Empty<string>();
        }
    }

    private static ServiceRegistration MapToServiceRegistration(ServiceEntry entry)
    {
        return new ServiceRegistration
        {
            ServiceId = entry.Service.ID,
            ServiceName = entry.Service.Service,
            Address = entry.Service.Address,
            Port = entry.Service.Port,
            Tags = entry.Service.Tags ?? Array.Empty<string>(),
            Metadata = entry.Service.Meta != null
                ? new Dictionary<string, string>(entry.Service.Meta)
                : new Dictionary<string, string>(),
            HealthStatus = entry.Checks.All(c => c.Status == HealthStatus.Passing) ? "Healthy" : "Unhealthy"
        };
    }
}
