namespace WSharp.Distributed.Resilience;

/// <summary>
/// 弹性策略配置选项
/// </summary>
public class ResilienceOptions
{
    /// <summary>
    /// 重试策略配置
    /// </summary>
    public RetryPolicyOptions? Retry { get; set; }

    /// <summary>
    /// 熔断器策略配置
    /// </summary>
    public CircuitBreakerPolicyOptions? CircuitBreaker { get; set; }

    /// <summary>
    /// 超时策略配置
    /// </summary>
    public TimeoutPolicyOptions? Timeout { get; set; }
}

/// <summary>
/// 重试策略配置
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// 重试延迟（毫秒）
    /// </summary>
    public int DelayMilliseconds { get; set; } = 1000;

    /// <summary>
    /// 是否使用指数退避
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// 是否使用抖动
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// 最大延迟时间（毫秒）
    /// </summary>
    public int MaxDelayMilliseconds { get; set; } = 30000;
}

/// <summary>
/// 熔断器策略配置
/// </summary>
public class CircuitBreakerPolicyOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 故障阈值百分比 (0-1)
    /// </summary>
    public double FailureThreshold { get; set; } = 0.5;

    /// <summary>
    /// 最小吞吐量（在采样期内的最小请求数）
    /// </summary>
    public int MinimumThroughput { get; set; } = 10;

    /// <summary>
    /// 采样持续时间（秒）
    /// </summary>
    public int SamplingDurationSeconds { get; set; } = 30;

    /// <summary>
    /// 熔断持续时间（秒）
    /// </summary>
    public int BreakDurationSeconds { get; set; } = 30;
}

/// <summary>
/// 超时策略配置
/// </summary>
public class TimeoutPolicyOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
