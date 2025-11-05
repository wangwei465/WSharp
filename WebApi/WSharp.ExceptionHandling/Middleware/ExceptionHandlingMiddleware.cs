using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WSharp.ExceptionHandling.Exceptions;
using WSharp.ExceptionHandling.Models;

namespace WSharp.ExceptionHandling.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var response = CreateErrorResponse(context, exception);
        var (statusCode, errorCode) = GetStatusCodeAndErrorCode(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
    {
        var response = new ErrorResponse
        {
            Message = GetUserFriendlyMessage(exception),
            ErrorCode = GetErrorCode(exception),
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path
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

    private (HttpStatusCode statusCode, string errorCode) GetStatusCodeAndErrorCode(Exception exception)
    {
        return exception switch
        {
            BusinessException businessEx => (businessEx.StatusCode, businessEx.ErrorCode),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR")
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
