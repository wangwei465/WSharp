using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WSharp.WebApi.Filters;

/// <summary>
/// 全局异常过滤器
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    /// <summary>
    /// 初始化异常过滤器
    /// </summary>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 异常处理
    /// </summary>
    public void OnException(ExceptionContext context)
    {
        this._logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

        var response = ApiResponse.Fail(
            message: "服务器内部错误",
            errorCode: "INTERNAL_SERVER_ERROR");

        response.TraceId = context.HttpContext.TraceIdentifier;

        // 开发环境返回详细错误信息
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isDevelopment)
        {
            response.Data = new
            {
                ExceptionType = context.Exception.GetType().Name,
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }
}

/// <summary>
/// 异步全局异常过滤器
/// </summary>
public class AsyncGlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<AsyncGlobalExceptionFilter> _logger;

    /// <summary>
    /// 初始化异常过滤器
    /// </summary>
    public AsyncGlobalExceptionFilter(ILogger<AsyncGlobalExceptionFilter> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 异步异常处理
    /// </summary>
    public Task OnExceptionAsync(ExceptionContext context)
    {
        this._logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

        var response = ApiResponse.Fail(
            message: this.GetUserFriendlyMessage(context.Exception),
            errorCode: this.GetErrorCode(context.Exception));

        response.TraceId = context.HttpContext.TraceIdentifier;

        // 开发环境返回详细错误信息
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isDevelopment)
        {
            response.Data = new
            {
                ExceptionType = context.Exception.GetType().Name,
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                InnerException = context.Exception.InnerException?.Message
            };
        }

        var statusCode = this.GetHttpStatusCode(context.Exception);
        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取用户友好的错误消息
    /// </summary>
    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "请求参数不能为空",
            ArgumentException => "请求参数错误",
            InvalidOperationException => "操作无效",
            UnauthorizedAccessException => "未授权访问",
            NotImplementedException => "功能尚未实现",
            TimeoutException => "请求超时",
            _ => "服务器内部错误"
        };
    }

    /// <summary>
    /// 获取错误代码
    /// </summary>
    private string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "ARGUMENT_NULL",
            ArgumentException => "INVALID_ARGUMENT",
            InvalidOperationException => "INVALID_OPERATION",
            UnauthorizedAccessException => "UNAUTHORIZED",
            NotImplementedException => "NOT_IMPLEMENTED",
            TimeoutException => "TIMEOUT",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    /// <summary>
    /// 获取 HTTP 状态码
    /// </summary>
    private int GetHttpStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => 400,
            ArgumentException => 400,
            InvalidOperationException => 400,
            UnauthorizedAccessException => 401,
            NotImplementedException => 501,
            TimeoutException => 408,
            _ => 500
        };
    }
}
