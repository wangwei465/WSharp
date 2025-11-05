namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Service discovery configuration options
/// </summary>
public class ServiceDiscoveryOptions
{
    /// <summary>
    /// Consul server address
    /// </summary>
    public string ConsulAddress { get; set; } = "http://localhost:8500";

    /// <summary>
    /// Service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Service ID (unique identifier)
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Service host address
    /// </summary>
    public string ServiceAddress { get; set; } = string.Empty;

    /// <summary>
    /// Service port
    /// </summary>
    public int ServicePort { get; set; }

    /// <summary>
    /// Service tags for metadata
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Health check endpoint
    /// </summary>
    public string? HealthCheckEndpoint { get; set; }

    /// <summary>
    /// Health check interval in seconds
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// Health check timeout in seconds
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Deregister critical service after (e.g., "30s", "1m")
    /// </summary>
    public string DeregisterCriticalServiceAfter { get; set; } = "30s";

    /// <summary>
    /// Enable automatic service registration on startup
    /// </summary>
    public bool EnableAutoRegistration { get; set; } = true;

    /// <summary>
    /// Enable automatic service deregistration on shutdown
    /// </summary>
    public bool EnableAutoDeregistration { get; set; } = true;

    /// <summary>
    /// Service metadata
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
