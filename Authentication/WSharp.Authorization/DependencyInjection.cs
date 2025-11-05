using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Authorization;

/// <summary>
/// 授权依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 授权
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpAuthorization(this IServiceCollection services)
    {
        // 注册授权处理器
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ResourceOperationAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, MultiRoleAuthorizationHandler>();

        // 配置授权策略
        services.AddAuthorization(options =>
        {
            // 角色策略
            options.AddPolicy(PolicyNames.RequireSuperAdmin, policy =>
                policy.RequireRole(PermissionConstants.SuperAdmin));

            options.AddPolicy(PolicyNames.RequireAdmin, policy =>
                policy.RequireRole(PermissionConstants.Admin));

            options.AddPolicy(PolicyNames.RequireUser, policy =>
                policy.RequireRole(PermissionConstants.User));

            // 基于资源的授权策略（占位符）
            options.AddPolicy(PolicyNames.ResourceBased, policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }

    /// <summary>
    /// 添加 WSharp 授权（带自定义策略）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configurePolicies">配置策略</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddWSharpAuthorization(
        this IServiceCollection services,
        Action<AuthorizationOptions> configurePolicies)
    {
        // 先添加基础授权
        services.AddWSharpAuthorization();

        // 应用自定义策略配置
        services.AddAuthorization(configurePolicies);

        return services;
    }

    /// <summary>
    /// 添加权限策略
    /// </summary>
    /// <param name="options">授权选项</param>
    /// <param name="permission">权限名称</param>
    /// <returns>授权选项</returns>
    public static AuthorizationOptions AddPermissionPolicy(
        this AuthorizationOptions options,
        string permission)
    {
        options.AddPolicy($"Permission:{permission}", policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));

        return options;
    }

    /// <summary>
    /// 添加资源操作策略
    /// </summary>
    /// <param name="options">授权选项</param>
    /// <param name="resourceType">资源类型</param>
    /// <param name="operation">操作</param>
    /// <returns>授权选项</returns>
    public static AuthorizationOptions AddResourceOperationPolicy(
        this AuthorizationOptions options,
        string resourceType,
        string operation)
    {
        options.AddPolicy($"Resource:{resourceType}:{operation}", policy =>
            policy.Requirements.Add(new ResourceOperationRequirement(resourceType, operation)));

        return options;
    }

    /// <summary>
    /// 添加多角色策略
    /// </summary>
    /// <param name="options">授权选项</param>
    /// <param name="requireAll">是否需要所有角色</param>
    /// <param name="roles">角色列表</param>
    /// <returns>授权选项</returns>
    public static AuthorizationOptions AddMultiRolePolicy(
        this AuthorizationOptions options,
        bool requireAll,
        params string[] roles)
    {
        var prefix = requireAll ? "AllRoles" : "AnyRole";
        var policyName = $"{prefix}:{string.Join(",", roles)}";

        options.AddPolicy(policyName, policy =>
            policy.Requirements.Add(new MultiRoleRequirement(roles, requireAll)));

        return options;
    }

    /// <summary>
    /// 批量添加权限策略
    /// </summary>
    /// <param name="options">授权选项</param>
    /// <param name="permissions">权限列表</param>
    /// <returns>授权选项</returns>
    public static AuthorizationOptions AddPermissionPolicies(
        this AuthorizationOptions options,
        params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            options.AddPermissionPolicy(permission);
        }

        return options;
    }

    /// <summary>
    /// 批量添加资源操作策略
    /// </summary>
    /// <param name="options">授权选项</param>
    /// <param name="resourceType">资源类型</param>
    /// <param name="operations">操作列表</param>
    /// <returns>授权选项</returns>
    public static AuthorizationOptions AddResourceOperationPolicies(
        this AuthorizationOptions options,
        string resourceType,
        params string[] operations)
    {
        foreach (var operation in operations)
        {
            options.AddResourceOperationPolicy(resourceType, operation);
        }

        return options;
    }
}
