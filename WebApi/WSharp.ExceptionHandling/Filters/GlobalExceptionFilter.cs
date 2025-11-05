using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WSharp.ExceptionHandling.Exceptions;
using WSharp.ExceptionHandling.Models;

namespace WSharp.ExceptionHandling.Filters;

/// <summary>
/// 全局异常过滤器
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

        var errorResponse = CreateErrorResponse(context);
        var statusCode = GetStatusCode(context.Exception);

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = (int)statusCode
        };

        context.ExceptionHandled = true;
    }

    private ErrorResponse CreateErrorResponse(ExceptionContext context)
    {
        var exception = context.Exception;
        var httpContext = context.HttpContext;

        var response = new ErrorResponse
        {
            Message = GetUserFriendlyMessage(exception),
            ErrorCode = GetErrorCode(exception),
            Timestamp = DateTime.UtcNow,
            TraceId = httpContext.TraceIdentifier,
            Path = httpContext.Request.Path
        };

        // 在开发环境下包含详细错误信息
        if (_environment.IsDevelopment())
        {
            response.Details = exception.Message;
            response.StackTrace = exception.StackTrace;
        }

        // 处理验证异常
        if (exception is ValidationException validationEx)
        {
            response.ValidationErrors = validationEx.Errors;
        }

        // 处理业务异常的额外详情
        if (exception is BusinessException businessEx && businessEx.Details != null)
        {
            response.AdditionalData = businessEx.Details;
        }

        return response;
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => businessEx.StatusCode,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => businessEx.ErrorCode,
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            BusinessException => exception.Message,
            _ when _environment.IsDevelopment() => exception.Message,
            _ => "An internal server error occurred. Please try again later."
        };
    }
}
