namespace WSharp.Gateway.Configuration;

/// <summary>
/// Gateway cluster (backend service) configuration
/// </summary>
public class GatewayClusterConfig
{
    /// <summary>
    /// Cluster ID (unique identifier)
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// Cluster destinations (backend services)
    /// </summary>
    public Dictionary<string, DestinationConfig> Destinations { get; set; } = new();

    /// <summary>
    /// Load balancing policy
    /// </summary>
    public string? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// Health check configuration
    /// </summary>
    public HealthCheckConfig? HealthCheck { get; set; }

    /// <summary>
    /// HTTP client configuration
    /// </summary>
    public HttpClientConfig? HttpClient { get; set; }
}

/// <summary>
/// Destination (backend service instance) configuration
/// </summary>
public class DestinationConfig
{
    /// <summary>
    /// Destination address (URL)
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Destination health endpoint
    /// </summary>
    public string? Health { get; set; }

    /// <summary>
    /// Destination metadata
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Health check configuration
/// </summary>
public class HealthCheckConfig
{
    /// <summary>
    /// Enable active health checks
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Health check interval
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Health check timeout
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Health check path
    /// </summary>
    public string Path { get; set; } = "/health";
}

/// <summary>
/// HTTP client configuration
/// </summary>
public class HttpClientConfig
{
    /// <summary>
    /// Request timeout
    /// </summary>
    public TimeSpan? RequestTimeout { get; set; }

    /// <summary>
    /// Maximum automatic redirections
    /// </summary>
    public int? MaxAutomaticRedirections { get; set; }

    /// <summary>
    /// Dangerous: Accept any SSL certificate
    /// </summary>
    public bool DangerouslyAcceptAnyServerCertificate { get; set; } = false;
}
