using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WSharp.Infrastructure.Logging;

/// <summary>
/// 日志辅助类
/// </summary>
public static class LogHelper
{
    /// <summary>
    /// 记录性能日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作名称</param>
    /// <param name="action">操作</param>
    /// <param name="properties">附加属性</param>
    public static void LogPerformance(
        this ILogger logger,
        string operation,
        Action action,
        Dictionary<string, object>? properties = null)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            action();
            sw.Stop();

            var message = $"Performance: {operation} completed in {sw.ElapsedMilliseconds}ms";
            if (properties != null)
            {
                logger.LogInformation(message + " {@Properties}", properties);
            }
            else
            {
                logger.LogInformation(message);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, $"Performance: {operation} failed after {sw.ElapsedMilliseconds}ms");
            throw;
        }
    }

    /// <summary>
    /// 记录性能日志（异步）
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作名称</param>
    /// <param name="action">操作</param>
    /// <param name="properties">附加属性</param>
    public static async Task LogPerformanceAsync(
        this ILogger logger,
        string operation,
        Func<Task> action,
        Dictionary<string, object>? properties = null)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await action();
            sw.Stop();

            var message = $"Performance: {operation} completed in {sw.ElapsedMilliseconds}ms";
            if (properties != null)
            {
                logger.LogInformation(message + " {@Properties}", properties);
            }
            else
            {
                logger.LogInformation(message);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, $"Performance: {operation} failed after {sw.ElapsedMilliseconds}ms");
            throw;
        }
    }

    /// <summary>
    /// 记录结构化日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="level">日志级别</param>
    /// <param name="message">消息</param>
    /// <param name="properties">属性</param>
    public static void LogStructured(
        this ILogger logger,
        LogLevel level,
        string message,
        Dictionary<string, object> properties)
    {
        logger.Log(level, message + " {@Properties}", properties);
    }

    /// <summary>
    /// 记录方法进入
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="methodName">方法名称</param>
    /// <param name="parameters">参数</param>
    public static void LogMethodEntry(
        this ILogger logger,
        string methodName,
        Dictionary<string, object>? parameters = null)
    {
        if (parameters != null)
        {
            logger.LogDebug("Entering {MethodName} with parameters {@Parameters}", methodName, parameters);
        }
        else
        {
            logger.LogDebug("Entering {MethodName}", methodName);
        }
    }

    /// <summary>
    /// 记录方法退出
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="methodName">方法名称</param>
    /// <param name="result">结果</param>
    public static void LogMethodExit(
        this ILogger logger,
        string methodName,
        object? result = null)
    {
        if (result != null)
        {
            logger.LogDebug("Exiting {MethodName} with result {@Result}", methodName, result);
        }
        else
        {
            logger.LogDebug("Exiting {MethodName}", methodName);
        }
    }

    /// <summary>
    /// 记录业务操作
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作</param>
    /// <param name="entity">实体</param>
    /// <param name="entityId">实体 ID</param>
    /// <param name="additionalInfo">附加信息</param>
    public static void LogBusinessOperation(
        this ILogger logger,
        string operation,
        string entity,
        string entityId,
        Dictionary<string, object>? additionalInfo = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["Operation"] = operation,
            ["Entity"] = entity,
            ["EntityId"] = entityId
        };

        if (additionalInfo != null)
        {
            foreach (var kvp in additionalInfo)
            {
                properties[kvp.Key] = kvp.Value;
            }
        }

        logger.LogInformation("Business operation: {Operation} on {Entity} {EntityId} {@Properties}",
            operation, entity, entityId, properties);
    }
}
