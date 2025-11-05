namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Service discovery interface
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// Register a service
    /// </summary>
    Task<bool> RegisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregister a service
    /// </summary>
    Task<bool> DeregisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all healthy service instances by name
    /// </summary>
    Task<IEnumerable<ServiceRegistration>> GetHealthyServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all service instances by name (including unhealthy)
    /// </summary>
    Task<IEnumerable<ServiceRegistration>> GetAllServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single service instance using load balancing
    /// </summary>
    Task<ServiceRegistration?> GetServiceAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all registered service names
    /// </summary>
    Task<IEnumerable<string>> GetAllServiceNamesAsync(CancellationToken cancellationToken = default);
}
