namespace WSharp.Gateway;

/// <summary>
/// Gateway configuration options
/// </summary>
public class GatewayOptions
{
    /// <summary>
    /// Enable request/response logging
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Enable distributed tracing
    /// </summary>
    public bool EnableTracing { get; set; } = true;

    /// <summary>
    /// Enable rate limiting
    /// </summary>
    public bool EnableRateLimiting { get; set; } = false;

    /// <summary>
    /// Rate limit requests per minute
    /// </summary>
    public int RateLimitPerMinute { get; set; } = 1000;

    /// <summary>
    /// Enable authentication
    /// </summary>
    public bool EnableAuthentication { get; set; } = false;

    /// <summary>
    /// JWT secret key for authentication
    /// </summary>
    public string? JwtSecretKey { get; set; }

    /// <summary>
    /// Enable load balancing
    /// </summary>
    public bool EnableLoadBalancing { get; set; } = true;

    /// <summary>
    /// Load balancing policy (FirstHealthy, Random, RoundRobin, LeastRequests, PowerOfTwoChoices)
    /// </summary>
    public string LoadBalancingPolicy { get; set; } = "RoundRobin";

    /// <summary>
    /// Enable health checks
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// Health check interval in seconds
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Timeout for health checks in seconds
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Enable response caching
    /// </summary>
    public bool EnableResponseCaching { get; set; } = false;

    /// <summary>
    /// Response cache duration in seconds
    /// </summary>
    public int ResponseCacheDurationSeconds { get; set; } = 60;
}
