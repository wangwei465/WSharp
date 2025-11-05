namespace WSharp.Gateway.Configuration;

/// <summary>
/// Gateway route configuration
/// </summary>
public class GatewayRouteConfig
{
    /// <summary>
    /// Route ID (unique identifier)
    /// </summary>
    public string RouteId { get; set; } = string.Empty;

    /// <summary>
    /// Cluster ID to route to
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// Route match pattern
    /// </summary>
    public string Match { get; set; } = string.Empty;

    /// <summary>
    /// Route order (lower values are matched first)
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Require authentication for this route
    /// </summary>
    public bool RequireAuthentication { get; set; } = false;

    /// <summary>
    /// Required roles for this route
    /// </summary>
    public string[]? RequiredRoles { get; set; }

    /// <summary>
    /// Enable rate limiting for this route
    /// </summary>
    public bool EnableRateLimiting { get; set; } = false;

    /// <summary>
    /// Rate limit per minute for this route
    /// </summary>
    public int? RateLimitPerMinute { get; set; }

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// Custom headers to add to the request
    /// </summary>
    public Dictionary<string, string>? RequestHeaders { get; set; }

    /// <summary>
    /// Headers to remove from the request
    /// </summary>
    public string[]? RemoveRequestHeaders { get; set; }

    /// <summary>
    /// Custom headers to add to the response
    /// </summary>
    public Dictionary<string, string>? ResponseHeaders { get; set; }

    /// <summary>
    /// Headers to remove from the response
    /// </summary>
    public string[]? RemoveResponseHeaders { get; set; }
}
