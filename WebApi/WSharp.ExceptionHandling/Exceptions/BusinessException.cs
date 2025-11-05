using System.Net;

namespace WSharp.ExceptionHandling.Exceptions;

/// <summary>
/// 业务异常基类
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// HTTP 状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// 额外的错误详情
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }

    public BusinessException(
        string message,
        string errorCode = "BUSINESS_ERROR",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public BusinessException(
        string message,
        Exception innerException,
        string errorCode = "BUSINESS_ERROR",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    /// <summary>
    /// 添加错误详情
    /// </summary>
    public BusinessException WithDetails(string key, object value)
    {
        Details ??= new Dictionary<string, object>();
        Details[key] = value;
        return this;
    }
}
