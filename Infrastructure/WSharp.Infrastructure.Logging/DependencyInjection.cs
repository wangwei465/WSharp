using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace WSharp.Infrastructure.Logging;

/// <summary>
/// 日志依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Serilog 日志
    /// </summary>
    /// <param name="builder">Web 应用程序构建器</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>Web 应用程序构建器</returns>
    public static WebApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder,
        Action<LoggingOptions>? configureOptions = null)
    {
        var options = new LoggingOptions();
        configureOptions?.Invoke(options);

        // 配置 Serilog
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            ConfigureSerilog(configuration, options, context.Configuration);
        });

        return builder;
    }

    /// <summary>
    /// 添加 Serilog 日志（使用 IHostBuilder）
    /// </summary>
    /// <param name="builder">主机构建器</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>主机构建器</returns>
    public static IHostBuilder AddSerilogLogging(
        this IHostBuilder builder,
        Action<LoggingOptions>? configureOptions = null)
    {
        var options = new LoggingOptions();
        configureOptions?.Invoke(options);

        builder.UseSerilog((context, services, configuration) =>
        {
            ConfigureSerilog(configuration, options, context.Configuration);
        });

        return builder;
    }

    /// <summary>
    /// 配置 Serilog
    /// </summary>
    private static void ConfigureSerilog(
        LoggerConfiguration loggerConfiguration,
        LoggingOptions options,
        IConfiguration configuration)
    {
        // 设置最小日志级别
        var minimumLevel = Enum.Parse<LogEventLevel>(options.MinimumLevel, true);
        loggerConfiguration
            .MinimumLevel.Is(minimumLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning);

        // 添加增强器
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", options.ApplicationName);

        // 控制台输出
        if (options.WriteToConsole)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        }

        // 文件输出
        if (options.WriteToFile)
        {
            var rollingInterval = ConvertRollingInterval(options.RollingInterval);
            loggerConfiguration.WriteTo.File(
                path: options.FilePath,
                rollingInterval: rollingInterval,
                retainedFileCountLimit: options.RetainedFileCountLimit,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        }

        // Elasticsearch 输出
        if (options.WriteToElasticsearch && !string.IsNullOrEmpty(options.ElasticsearchUri))
        {
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.ElasticsearchUri))
            {
                IndexFormat = options.ElasticsearchIndexFormat,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                FailureCallback = e => Console.WriteLine($"Unable to submit event to Elasticsearch: {e.MessageTemplate}"),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                   EmitEventFailureHandling.WriteToFailureSink |
                                   EmitEventFailureHandling.RaiseCallback
            });
        }

        // 从配置文件读取额外配置
        loggerConfiguration.ReadFrom.Configuration(configuration);
    }

    /// <summary>
    /// 转换滚动间隔
    /// </summary>
    private static Serilog.RollingInterval ConvertRollingInterval(RollingInterval interval)
    {
        return interval switch
        {
            RollingInterval.Second => Serilog.RollingInterval.Minute,
            RollingInterval.Minute => Serilog.RollingInterval.Hour,
            RollingInterval.Hour => Serilog.RollingInterval.Hour,
            RollingInterval.Day => Serilog.RollingInterval.Day,
            RollingInterval.Month => Serilog.RollingInterval.Month,
            RollingInterval.Year => Serilog.RollingInterval.Year,
            _ => Serilog.RollingInterval.Day
        };
    }

    /// <summary>
    /// 使用 Serilog 请求日志
    /// </summary>
    /// <param name="app">应用程序</param>
    /// <returns>应用程序</returns>
    public static WebApplication UseSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            };
        });

        return app;
    }
}
