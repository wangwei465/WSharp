namespace WSharp.Distributed.Tracing;

/// <summary>
/// OpenTelemetry 追踪配置选项
/// </summary>
public class TracingOptions
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; } = "WSharp.Service";

    /// <summary>
    /// 服务版本
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    /// 是否启用追踪
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 采样率 (0.0 - 1.0)
    /// </summary>
    public double SamplingRate { get; set; } = 1.0;

    /// <summary>
    /// 是否启用 ASP.NET Core 自动追踪
    /// </summary>
    public bool EnableAspNetCoreInstrumentation { get; set; } = true;

    /// <summary>
    /// 是否启用 HttpClient 自动追踪
    /// </summary>
    public bool EnableHttpClientInstrumentation { get; set; } = true;

    /// <summary>
    /// 是否启用 SQL Client 自动追踪
    /// </summary>
    public bool EnableSqlClientInstrumentation { get; set; } = true;

    /// <summary>
    /// 是否导出到控制台
    /// </summary>
    public bool EnableConsoleExporter { get; set; } = false;

    /// <summary>
    /// Jaeger 导出器配置
    /// </summary>
    public JaegerExporterOptions? Jaeger { get; set; }

    /// <summary>
    /// Zipkin 导出器配置
    /// </summary>
    public ZipkinExporterOptions? Zipkin { get; set; }

    /// <summary>
    /// OTLP (OpenTelemetry Protocol) 导出器配置
    /// </summary>
    public OtlpExporterOptions? Otlp { get; set; }

    /// <summary>
    /// 要追踪的源名称列表
    /// </summary>
    public List<string> Sources { get; set; } = new();
}

/// <summary>
/// Jaeger 导出器配置
/// </summary>
public class JaegerExporterOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Agent 主机
    /// </summary>
    public string AgentHost { get; set; } = "localhost";

    /// <summary>
    /// Agent 端口
    /// </summary>
    public int AgentPort { get; set; } = 6831;

    /// <summary>
    /// 最大数据包大小
    /// </summary>
    public int? MaxPacketSize { get; set; }
}

/// <summary>
/// Zipkin 导出器配置
/// </summary>
public class ZipkinExporterOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Zipkin 端点 URL
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:9411/api/v2/spans";

    /// <summary>
    /// 是否使用短 Trace ID
    /// </summary>
    public bool UseShortTraceIds { get; set; }
}

/// <summary>
/// OTLP 导出器配置
/// </summary>
public class OtlpExporterOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// OTLP 端点 URL
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:4317";

    /// <summary>
    /// 导出协议 (Grpc 或 HttpProtobuf)
    /// </summary>
    public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.Grpc;

    /// <summary>
    /// 请求头
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();
}

/// <summary>
/// OTLP 导出协议
/// </summary>
public enum OtlpExportProtocol
{
    /// <summary>
    /// gRPC 协议
    /// </summary>
    Grpc,

    /// <summary>
    /// HTTP/Protobuf 协议
    /// </summary>
    HttpProtobuf
}
