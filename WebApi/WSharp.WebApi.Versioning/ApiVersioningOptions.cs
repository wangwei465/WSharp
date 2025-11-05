using Microsoft.AspNetCore.Mvc;

namespace WSharp.WebApi.Versioning;

/// <summary>
/// API 版本控制配置选项
/// </summary>
public class ApiVersioningOptions
{
    /// <summary>
    /// 默认 API 版本
    /// </summary>
    public ApiVersion DefaultApiVersion { get; set; } = new ApiVersion(1, 0);

    /// <summary>
    /// 当请求未指定版本时，是否使用默认版本
    /// </summary>
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;

    /// <summary>
    /// 是否在响应头中报告支持的 API 版本
    /// </summary>
    public bool ReportApiVersions { get; set; } = true;

    /// <summary>
    /// 是否启用 URL 路径段版本控制 (例如: /api/v1/products)
    /// </summary>
    public bool EnableUrlSegmentVersioning { get; set; } = true;

    /// <summary>
    /// 是否启用查询字符串版本控制 (例如: /api/products?api-version=1.0)
    /// </summary>
    public bool EnableQueryStringVersioning { get; set; } = false;

    /// <summary>
    /// 是否启用 HTTP 头版本控制 (例如: api-version: 1.0)
    /// </summary>
    public bool EnableHeaderVersioning { get; set; } = false;

    /// <summary>
    /// 是否启用媒体类型版本控制 (例如: Accept: application/json;v=1.0)
    /// </summary>
    public bool EnableMediaTypeVersioning { get; set; } = false;

    /// <summary>
    /// 查询字符串参数名称
    /// </summary>
    public string QueryStringParameterName { get; set; } = "api-version";

    /// <summary>
    /// HTTP 头名称
    /// </summary>
    public string HeaderName { get; set; } = "api-version";

    /// <summary>
    /// 媒体类型参数名称
    /// </summary>
    public string MediaTypeParameterName { get; set; } = "v";

    /// <summary>
    /// 版本格式 (Major, MajorMinor, GroupVersion)
    /// </summary>
    public ApiVersionFormatType VersionFormat { get; set; } = ApiVersionFormatType.MajorMinor;

    /// <summary>
    /// 是否启用 API 版本的详细信息探索
    /// </summary>
    public bool EnableApiExplorer { get; set; } = true;

    /// <summary>
    /// API 版本分组名称格式 (例如: "'v'VVV" => v1, v2)
    /// </summary>
    public string GroupNameFormat { get; set; } = "'v'VVV";

    /// <summary>
    /// 是否替换默认的版本格式
    /// </summary>
    public bool SubstituteApiVersionInUrl { get; set; } = true;
}

/// <summary>
/// API 版本格式类型
/// </summary>
public enum ApiVersionFormatType
{
    /// <summary>
    /// 仅主版本号 (例如: 1)
    /// </summary>
    Major,

    /// <summary>
    /// 主版本号和次版本号 (例如: 1.0)
    /// </summary>
    MajorMinor,

    /// <summary>
    /// 分组版本 (例如: v1)
    /// </summary>
    GroupVersion
}
