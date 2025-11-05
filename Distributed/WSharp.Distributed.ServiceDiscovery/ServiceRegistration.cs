namespace WSharp.Distributed.ServiceDiscovery;

/// <summary>
/// Service registration information
/// </summary>
public class ServiceRegistration
{
    /// <summary>
    /// Service ID
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Service address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Service port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Service tags
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Service metadata
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Health status
    /// </summary>
    public string HealthStatus { get; set; } = "Unknown";

    /// <summary>
    /// Full service URL
    /// </summary>
    public string ServiceUrl => $"{Address}:{Port}";
}
