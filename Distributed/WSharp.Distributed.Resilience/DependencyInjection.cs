using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

namespace WSharp.Distributed.Resilience;

/// <summary>
/// 弹性策略依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 弹性策略
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpResilience(
        this IServiceCollection services,
        Action<ResilienceOptions>? configureOptions = null)
    {
        var options = new ResilienceOptions
        {
            Retry = new RetryPolicyOptions(),
            CircuitBreaker = new CircuitBreakerPolicyOptions(),
            Timeout = new TimeoutPolicyOptions()
        };
        configureOptions?.Invoke(options);

        services.AddSingleton(options);

        // 构建并注册弹性策略
        var asyncPolicy = ResiliencePolicyBuilder.BuildAsyncPolicy(options);
        var syncPolicy = ResiliencePolicyBuilder.BuildSyncPolicy(options);

        services.AddSingleton(asyncPolicy);
        services.AddSingleton(syncPolicy);
        services.AddSingleton(sp => new ResilienceExecutor(
            sp.GetRequiredService<IAsyncPolicy>(),
            sp.GetRequiredService<ISyncPolicy>()));

        return services;
    }

    /// <summary>
    /// 为 HttpClient 添加弹性策略
    /// </summary>
    /// <param name="builder">HttpClient 构建器</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>HttpClient 构建器</returns>
    public static IHttpClientBuilder AddWSharpResilience(
        this IHttpClientBuilder builder,
        Action<ResilienceOptions>? configureOptions = null)
    {
        var options = new ResilienceOptions
        {
            Retry = new RetryPolicyOptions(),
            CircuitBreaker = new CircuitBreakerPolicyOptions(),
            Timeout = new TimeoutPolicyOptions()
        };
        configureOptions?.Invoke(options);

        // 添加重试策略
        if (options.Retry?.Enabled == true)
        {
            if (options.Retry.UseExponentialBackoff)
            {
                builder.AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(Math.Min(
                                options.Retry.DelayMilliseconds * Math.Pow(2, retryAttempt - 1),
                                options.Retry.MaxDelayMilliseconds))));
            }
            else
            {
                builder.AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retryCount: options.Retry.MaxRetryAttempts,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromMilliseconds(options.Retry.DelayMilliseconds)));
            }
        }

        // 添加熔断器策略
        if (options.CircuitBreaker?.Enabled == true)
        {
            builder.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: options.CircuitBreaker.FailureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(options.CircuitBreaker.SamplingDurationSeconds),
                    minimumThroughput: options.CircuitBreaker.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDurationSeconds)));
        }

        // 添加超时策略
        if (options.Timeout?.Enabled == true)
        {
            builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(options.Timeout.TimeoutSeconds)));
        }

        return builder;
    }
}
