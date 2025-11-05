using Microsoft.OpenApi.Models;

namespace WSharp.WebApi.Swagger;

/// <summary>
/// Swagger 配置选项
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// API 标题
    /// </summary>
    public string Title { get; set; } = "WSharp API";

    /// <summary>
    /// API 描述
    /// </summary>
    public string Description { get; set; } = "WSharp Framework API Documentation";

    /// <summary>
    /// API 版本
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// 联系人信息
    /// </summary>
    public OpenApiContact? Contact { get; set; }

    /// <summary>
    /// 许可证信息
    /// </summary>
    public OpenApiLicense? License { get; set; }

    /// <summary>
    /// 服务器列表
    /// </summary>
    public List<OpenApiServer> Servers { get; set; } = new();

    /// <summary>
    /// 是否启用 JWT Bearer 认证
    /// </summary>
    public bool EnableJwtBearer { get; set; } = true;

    /// <summary>
    /// 是否启用 OAuth2 认证
    /// </summary>
    public bool EnableOAuth2 { get; set; }

    /// <summary>
    /// OAuth2 授权 URL
    /// </summary>
    public string? OAuth2AuthorizationUrl { get; set; }

    /// <summary>
    /// OAuth2 令牌 URL
    /// </summary>
    public string? OAuth2TokenUrl { get; set; }

    /// <summary>
    /// OAuth2 作用域
    /// </summary>
    public Dictionary<string, string> OAuth2Scopes { get; set; } = new();

    /// <summary>
    /// 是否显示枚举为字符串
    /// </summary>
    public bool UseEnumsAsStrings { get; set; } = true;

    /// <summary>
    /// 是否使用驼峰命名
    /// </summary>
    public bool UseCamelCase { get; set; } = true;

    /// <summary>
    /// XML 文档路径列表
    /// </summary>
    public List<string> XmlDocumentPaths { get; set; } = new();

    /// <summary>
    /// Swagger UI 路由前缀
    /// </summary>
    public string RoutePrefix { get; set; } = "swagger";

    /// <summary>
    /// Swagger 文档名称
    /// </summary>
    public string DocumentName { get; set; } = "v1";
}
