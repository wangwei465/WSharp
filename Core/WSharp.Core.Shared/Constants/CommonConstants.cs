namespace WSharp.Core.Shared.Constants;

/// <summary>
/// 常用错误代码
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// 未知错误
    /// </summary>
    public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";

    /// <summary>
    /// 验证失败
    /// </summary>
    public const string VALIDATION_FAILED = "VALIDATION_FAILED";

    /// <summary>
    /// 未授权
    /// </summary>
    public const string UNAUTHORIZED = "UNAUTHORIZED";

    /// <summary>
    /// 禁止访问
    /// </summary>
    public const string FORBIDDEN = "FORBIDDEN";

    /// <summary>
    /// 未找到
    /// </summary>
    public const string NOT_FOUND = "NOT_FOUND";

    /// <summary>
    /// 冲突
    /// </summary>
    public const string CONFLICT = "CONFLICT";

    /// <summary>
    /// 内部服务器错误
    /// </summary>
    public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";

    /// <summary>
    /// 服务不可用
    /// </summary>
    public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";

    /// <summary>
    /// 请求超时
    /// </summary>
    public const string REQUEST_TIMEOUT = "REQUEST_TIMEOUT";

    /// <summary>
    /// 参数无效
    /// </summary>
    public const string INVALID_ARGUMENT = "INVALID_ARGUMENT";

    /// <summary>
    /// 数据库错误
    /// </summary>
    public const string DATABASE_ERROR = "DATABASE_ERROR";

    /// <summary>
    /// 外部服务错误
    /// </summary>
    public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";
}

/// <summary>
/// 常用正则表达式模式
/// </summary>
public static class RegexPatterns
{
    /// <summary>
    /// 电子邮件地址
    /// </summary>
    public const string EMAIL = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    /// <summary>
    /// 手机号码（中国大陆）
    /// </summary>
    public const string MOBILE_PHONE_CN = @"^1[3-9]\d{9}$";

    /// <summary>
    /// URL
    /// </summary>
    public const string URL = @"^https?://[^\s/$.?#].[^\s]*$";

    /// <summary>
    /// IPv4 地址
    /// </summary>
    public const string IPV4 = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

    /// <summary>
    /// IPv6 地址
    /// </summary>
    public const string IPV6 = @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$";

    /// <summary>
    /// 身份证号码（中国大陆）
    /// </summary>
    public const string ID_CARD_CN = @"^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$";

    /// <summary>
    /// 邮政编码（中国大陆）
    /// </summary>
    public const string POSTAL_CODE_CN = @"^[1-9]\d{5}$";

    /// <summary>
    /// 十六进制颜色代码
    /// </summary>
    public const string HEX_COLOR = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";

    /// <summary>
    /// 强密码（至少8位,包含大小写字母、数字和特殊字符）
    /// </summary>
    public const string STRONG_PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

    /// <summary>
    /// 用户名（4-16位字母、数字、下划线）
    /// </summary>
    public const string USERNAME = @"^[a-zA-Z0-9_]{4,16}$";
}

/// <summary>
/// 分页常量
/// </summary>
public static class PaginationConstants
{
    /// <summary>
    /// 默认页码
    /// </summary>
    public const int DEFAULT_PAGE_NUMBER = 1;

    /// <summary>
    /// 默认每页大小
    /// </summary>
    public const int DEFAULT_PAGE_SIZE = 10;

    /// <summary>
    /// 最大每页大小
    /// </summary>
    public const int MAX_PAGE_SIZE = 100;

    /// <summary>
    /// 最小每页大小
    /// </summary>
    public const int MIN_PAGE_SIZE = 1;
}

/// <summary>
/// HTTP 头部常量
/// </summary>
public static class HttpHeaders
{
    /// <summary>
    /// Authorization 头
    /// </summary>
    public const string AUTHORIZATION = "Authorization";

    /// <summary>
    /// Content-Type 头
    /// </summary>
    public const string CONTENT_TYPE = "Content-Type";

    /// <summary>
    /// Accept 头
    /// </summary>
    public const string ACCEPT = "Accept";

    /// <summary>
    /// User-Agent 头
    /// </summary>
    public const string USER_AGENT = "User-Agent";

    /// <summary>
    /// X-Request-ID 头
    /// </summary>
    public const string X_REQUEST_ID = "X-Request-ID";

    /// <summary>
    /// X-Correlation-ID 头
    /// </summary>
    public const string X_CORRELATION_ID = "X-Correlation-ID";

    /// <summary>
    /// X-Tenant-ID 头
    /// </summary>
    public const string X_TENANT_ID = "X-Tenant-ID";

    /// <summary>
    /// Bearer Token 前缀
    /// </summary>
    public const string BEARER_PREFIX = "Bearer ";
}

/// <summary>
/// 日期时间格式常量
/// </summary>
public static class DateTimeFormats
{
    /// <summary>
    /// ISO 8601 日期时间格式
    /// </summary>
    public const string ISO8601 = "yyyy-MM-ddTHH:mm:ss.fffZ";

    /// <summary>
    /// 标准日期格式
    /// </summary>
    public const string DATE = "yyyy-MM-dd";

    /// <summary>
    /// 标准时间格式
    /// </summary>
    public const string TIME = "HH:mm:ss";

    /// <summary>
    /// 标准日期时间格式
    /// </summary>
    public const string DATETIME = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// 中文日期格式
    /// </summary>
    public const string DATE_CN = "yyyy年MM月dd日";

    /// <summary>
    /// 中文日期时间格式
    /// </summary>
    public const string DATETIME_CN = "yyyy年MM月dd日 HH:mm:ss";
}
