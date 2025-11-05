using Serilog.Core;
using Serilog.Events;

namespace WSharp.Infrastructure.Logging;

/// <summary>
/// 关联 ID 增强器
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly string _correlationId;

    /// <summary>
    /// 初始化关联 ID 增强器
    /// </summary>
    /// <param name="correlationId">关联 ID</param>
    public CorrelationIdEnricher(string correlationId)
    {
        this._correlationId = correlationId;
    }

    /// <summary>
    /// 增强日志事件
    /// </summary>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var property = propertyFactory.CreateProperty("CorrelationId", this._correlationId);
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// 用户上下文增强器
/// </summary>
public class UserContextEnricher : ILogEventEnricher
{
    private readonly string? _userId;
    private readonly string? _userName;

    /// <summary>
    /// 初始化用户上下文增强器
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名</param>
    public UserContextEnricher(string? userId, string? userName)
    {
        this._userId = userId;
        this._userName = userName;
    }

    /// <summary>
    /// 增强日志事件
    /// </summary>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!string.IsNullOrEmpty(this._userId))
        {
            var userIdProperty = propertyFactory.CreateProperty("UserId", this._userId);
            logEvent.AddPropertyIfAbsent(userIdProperty);
        }

        if (!string.IsNullOrEmpty(this._userName))
        {
            var userNameProperty = propertyFactory.CreateProperty("UserName", this._userName);
            logEvent.AddPropertyIfAbsent(userNameProperty);
        }
    }
}

/// <summary>
/// 租户上下文增强器
/// </summary>
public class TenantContextEnricher : ILogEventEnricher
{
    private readonly string? _tenantId;

    /// <summary>
    /// 初始化租户上下文增强器
    /// </summary>
    /// <param name="tenantId">租户 ID</param>
    public TenantContextEnricher(string? tenantId)
    {
        this._tenantId = tenantId;
    }

    /// <summary>
    /// 增强日志事件
    /// </summary>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!string.IsNullOrEmpty(this._tenantId))
        {
            var property = propertyFactory.CreateProperty("TenantId", this._tenantId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
