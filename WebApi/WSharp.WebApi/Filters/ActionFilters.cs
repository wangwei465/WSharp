using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WSharp.WebApi.Filters;

/// <summary>
/// 模型验证过滤器
/// </summary>
public class ModelValidationFilter : IActionFilter
{
    /// <summary>
    /// Action 执行前
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Messages = e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                })
                .ToList();

            var response = ApiResponse.Fail(
                "请求参数验证失败",
                "VALIDATION_ERROR");

            response.Data = errors;

            context.Result = new BadRequestObjectResult(response);
        }
    }

    /// <summary>
    /// Action 执行后
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // 不需要处理
    }
}

/// <summary>
/// 响应包装过滤器
/// </summary>
public class ResponseWrapperFilter : IResultFilter
{
    /// <summary>
    /// Result 执行前
    /// </summary>
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // 只包装成功的结果
        if (context.Result is ObjectResult objectResult &&
            objectResult.StatusCode is >= 200 and < 300)
        {
            var value = objectResult.Value;

            // 如果已经是 ApiResponse，则不再包装
            if (value != null && value.GetType().IsGenericType &&
                value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                return;
            }

            if (value != null && value.GetType() == typeof(ApiResponse))
            {
                return;
            }

            // 包装成 ApiResponse
            var responseType = typeof(ApiResponse<>).MakeGenericType(value?.GetType() ?? typeof(object));
            var response = Activator.CreateInstance(responseType);

            if (response != null)
            {
                var successProperty = responseType.GetProperty("Success");
                var messageProperty = responseType.GetProperty("Message");
                var dataProperty = responseType.GetProperty("Data");
                var timestampProperty = responseType.GetProperty("Timestamp");
                var traceIdProperty = responseType.GetProperty("TraceId");

                successProperty?.SetValue(response, true);
                messageProperty?.SetValue(response, "操作成功");
                dataProperty?.SetValue(response, value);
                timestampProperty?.SetValue(response, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                traceIdProperty?.SetValue(response, context.HttpContext.TraceIdentifier);

                objectResult.Value = response;
            }
        }
    }

    /// <summary>
    /// Result 执行后
    /// </summary>
    public void OnResultExecuted(ResultExecutedContext context)
    {
        // 不需要处理
    }
}

/// <summary>
/// 性能监控过滤器
/// </summary>
public class PerformanceMonitorFilter : IActionFilter, IAsyncActionFilter
{
    private const string StopwatchKey = "PerformanceMonitor_Stopwatch";

    /// <summary>
    /// Action 执行前
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;
    }

    /// <summary>
    /// Action 执行后
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[StopwatchKey] is System.Diagnostics.Stopwatch stopwatch)
        {
            stopwatch.Stop();
            context.HttpContext.Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";
        }
    }

    /// <summary>
    /// 异步 Action 执行
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        this.OnActionExecuting(context);
        var resultContext = await next();
        this.OnActionExecuted(resultContext);
    }
}
