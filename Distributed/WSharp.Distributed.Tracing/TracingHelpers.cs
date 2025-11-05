using System.Diagnostics;

namespace WSharp.Distributed.Tracing;

/// <summary>
/// 追踪活动源
/// </summary>
public static class ActivitySources
{
    /// <summary>
    /// 默认活动源名称
    /// </summary>
    public const string DefaultSourceName = "WSharp";

    /// <summary>
    /// 默认活动源
    /// </summary>
    public static readonly ActivitySource Default = new(DefaultSourceName);

    /// <summary>
    /// 创建自定义活动源
    /// </summary>
    /// <param name="sourceName">源名称</param>
    /// <param name="version">版本</param>
    /// <returns>活动源</returns>
    public static ActivitySource Create(string sourceName, string? version = null)
    {
        return new ActivitySource(sourceName, version);
    }
}

/// <summary>
/// 追踪活动扩展方法
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// 设置标签
    /// </summary>
    public static Activity? SetTag(this Activity? activity, string key, object? value)
    {
        activity?.SetTag(key, value);
        return activity;
    }

    /// <summary>
    /// 设置多个标签
    /// </summary>
    public static Activity? SetTags(this Activity? activity, params (string key, object? value)[] tags)
    {
        if (activity == null) return null;

        foreach (var (key, value) in tags)
        {
            activity.SetTag(key, value);
        }

        return activity;
    }

    /// <summary>
    /// 记录异常
    /// </summary>
    public static Activity? RecordException(this Activity? activity, Exception exception)
    {
        if (activity == null) return null;

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.RecordException(exception);

        return activity;
    }

    /// <summary>
    /// 设置状态为成功
    /// </summary>
    public static Activity? SetSuccess(this Activity? activity)
    {
        activity?.SetStatus(ActivityStatusCode.Ok);
        return activity;
    }

    /// <summary>
    /// 设置状态为错误
    /// </summary>
    public static Activity? SetError(this Activity? activity, string description)
    {
        activity?.SetStatus(ActivityStatusCode.Error, description);
        return activity;
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    public static Activity? AddEvent(this Activity? activity, string eventName, params (string key, object? value)[] tags)
    {
        if (activity == null) return null;

        var tagList = new ActivityTagsCollection();
        foreach (var (key, value) in tags)
        {
            tagList.Add(key, value);
        }

        activity.AddEvent(new ActivityEvent(eventName, tags: tagList));
        return activity;
    }
}

/// <summary>
/// 追踪辅助类
/// </summary>
public static class TracingHelper
{
    /// <summary>
    /// 使用追踪执行操作
    /// </summary>
    /// <param name="activitySource">活动源</param>
    /// <param name="activityName">活动名称</param>
    /// <param name="action">要执行的操作</param>
    /// <param name="kind">活动类型</param>
    public static void WithActivity(
        ActivitySource activitySource,
        string activityName,
        Action action,
        ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = activitySource.StartActivity(activityName, kind);
        try
        {
            action();
            activity?.SetSuccess();
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// 使用追踪执行操作并返回结果
    /// </summary>
    public static T WithActivity<T>(
        ActivitySource activitySource,
        string activityName,
        Func<T> func,
        ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = activitySource.StartActivity(activityName, kind);
        try
        {
            var result = func();
            activity?.SetSuccess();
            return result;
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// 使用追踪执行异步操作
    /// </summary>
    public static async Task WithActivityAsync(
        ActivitySource activitySource,
        string activityName,
        Func<Task> asyncAction,
        ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = activitySource.StartActivity(activityName, kind);
        try
        {
            await asyncAction();
            activity?.SetSuccess();
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// 使用追踪执行异步操作并返回结果
    /// </summary>
    public static async Task<T> WithActivityAsync<T>(
        ActivitySource activitySource,
        string activityName,
        Func<Task<T>> asyncFunc,
        ActivityKind kind = ActivityKind.Internal)
    {
        using var activity = activitySource.StartActivity(activityName, kind);
        try
        {
            var result = await asyncFunc();
            activity?.SetSuccess();
            return result;
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }
}
