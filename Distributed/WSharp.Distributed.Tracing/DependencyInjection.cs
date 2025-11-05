using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WSharp.Distributed.Tracing;

/// <summary>
/// 分布式追踪依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 分布式追踪
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpTracing(
        this IServiceCollection services,
        Action<TracingOptions>? configureOptions = null)
    {
        var options = new TracingOptions();
        configureOptions?.Invoke(options);

        if (!options.Enabled)
        {
            return services;
        }

        services.AddSingleton(options);

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                // 配置资源
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: options.ServiceName,
                        serviceVersion: options.ServiceVersion));

                // 配置采样器
                if (options.SamplingRate < 1.0)
                {
                    builder.SetSampler(new TraceIdRatioBasedSampler(options.SamplingRate));
                }

                // 添加自定义源
                if (options.Sources.Any())
                {
                    builder.AddSource(options.Sources.ToArray());
                }

                // ASP.NET Core 自动追踪
                if (options.EnableAspNetCoreInstrumentation)
                {
                    builder.AddAspNetCoreInstrumentation(instrumentationOptions =>
                    {
                        instrumentationOptions.RecordException = true;
                    });
                }

                // HttpClient 自动追踪
                if (options.EnableHttpClientInstrumentation)
                {
                    builder.AddHttpClientInstrumentation(instrumentationOptions =>
                    {
                        instrumentationOptions.RecordException = true;
                    });
                }

                // SQL Client 自动追踪
                if (options.EnableSqlClientInstrumentation)
                {
                    builder.AddSqlClientInstrumentation(instrumentationOptions =>
                    {
                        instrumentationOptions.SetDbStatementForText = true;
                        instrumentationOptions.RecordException = true;
                    });
                }

                // 控制台导出器
                if (options.EnableConsoleExporter)
                {
                    builder.AddConsoleExporter();
                }

                // Jaeger 导出器
                if (options.Jaeger?.Enabled == true)
                {
                    builder.AddJaegerExporter(jaegerOptions =>
                    {
                        jaegerOptions.AgentHost = options.Jaeger.AgentHost;
                        jaegerOptions.AgentPort = options.Jaeger.AgentPort;
                        if (options.Jaeger.MaxPacketSize.HasValue)
                        {
                            jaegerOptions.MaxPayloadSizeInBytes = options.Jaeger.MaxPacketSize.Value;
                        }
                    });
                }

                // Zipkin 导出器
                if (options.Zipkin?.Enabled == true)
                {
                    builder.AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri(options.Zipkin.Endpoint);
                        zipkinOptions.UseShortTraceIds = options.Zipkin.UseShortTraceIds;
                    });
                }

                // OTLP 导出器
                if (options.Otlp?.Enabled == true)
                {
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.Otlp.Endpoint);
                        otlpOptions.Protocol = options.Otlp.Protocol == OtlpExportProtocol.Grpc
                            ? OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
                            : OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;

                        if (options.Otlp.Headers.Any())
                        {
                            otlpOptions.Headers = string.Join(",",
                                options.Otlp.Headers.Select(h => $"{h.Key}={h.Value}"));
                        }
                    });
                }
            });

        return services;
    }
}
