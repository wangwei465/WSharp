using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace WSharp.Distributed.Resilience;

/// <summary>
/// 弹性策略构建器
/// </summary>
public static class ResiliencePolicyBuilder
{
    /// <summary>
    /// 构建弹性策略
    /// </summary>
    /// <param name="options">配置选项</param>
    /// <returns>弹性策略</returns>
    public static IAsyncPolicy BuildAsyncPolicy(ResilienceOptions options)
    {
        var policies = new List<IAsyncPolicy>();

        // 添加重试策略（最内层）
        if (options.Retry?.Enabled == true)
        {
            var retryPolicy = options.Retry.UseExponentialBackoff
                ? Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(Math.Min(
                                options.Retry.DelayMilliseconds * Math.Pow(2, retryAttempt - 1),
                                options.Retry.MaxDelayMilliseconds)),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            // 可以添加日志记录
                        })
                : Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(options.Retry.DelayMilliseconds),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            // 可以添加日志记录
                        });

            policies.Add(retryPolicy);
        }

        // 添加熔断器策略
        if (options.CircuitBreaker?.Enabled == true)
        {
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: options.CircuitBreaker.FailureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(options.CircuitBreaker.SamplingDurationSeconds),
                    minimumThroughput: options.CircuitBreaker.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDurationSeconds),
                    onBreak: (exception, duration) =>
                    {
                        // 可以添加日志记录
                    },
                    onReset: () =>
                    {
                        // 可以添加日志记录
                    });

            policies.Add(circuitBreakerPolicy);
        }

        // 添加超时策略（最外层）
        if (options.Timeout?.Enabled == true)
        {
            var timeoutPolicy = Policy
                .TimeoutAsync(TimeSpan.FromSeconds(options.Timeout.TimeoutSeconds));

            policies.Add(timeoutPolicy);
        }

        // 组合所有策略
        return policies.Count switch
        {
            0 => Policy.NoOpAsync(),
            1 => policies[0],
            _ => Policy.WrapAsync(policies.ToArray())
        };
    }

    /// <summary>
    /// 构建同步弹性策略
    /// </summary>
    /// <param name="options">配置选项</param>
    /// <returns>弹性策略</returns>
    public static ISyncPolicy BuildSyncPolicy(ResilienceOptions options)
    {
        var policies = new List<ISyncPolicy>();

        // 添加重试策略
        if (options.Retry?.Enabled == true)
        {
            var retryPolicy = options.Retry.UseExponentialBackoff
                ? Policy
                    .Handle<Exception>()
                    .WaitAndRetry(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(Math.Min(
                                options.Retry.DelayMilliseconds * Math.Pow(2, retryAttempt - 1),
                                options.Retry.MaxDelayMilliseconds)))
                : Policy
                    .Handle<Exception>()
                    .WaitAndRetry(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(options.Retry.DelayMilliseconds));

            policies.Add(retryPolicy);
        }

        // 添加熔断器策略
        if (options.CircuitBreaker?.Enabled == true)
        {
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreaker(
                    failureThreshold: options.CircuitBreaker.FailureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(options.CircuitBreaker.SamplingDurationSeconds),
                    minimumThroughput: options.CircuitBreaker.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDurationSeconds));

            policies.Add(circuitBreakerPolicy);
        }

        // 添加超时策略
        if (options.Timeout?.Enabled == true)
        {
            var timeoutPolicy = Policy
                .Timeout(TimeSpan.FromSeconds(options.Timeout.TimeoutSeconds));

            policies.Add(timeoutPolicy);
        }

        // 组合所有策略
        return policies.Count switch
        {
            0 => Policy.NoOp(),
            1 => policies[0],
            _ => Policy.Wrap(policies.ToArray())
        };
    }
}
