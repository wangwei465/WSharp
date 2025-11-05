using Microsoft.AspNetCore.Authorization;

namespace WSharp.Authorization;

/// <summary>
/// 权限授权处理器
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// 处理权限要求
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 检查用户是否已认证
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // 超级管理员拥有所有权限
        if (context.User.IsInRole(PermissionConstants.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 检查用户是否拥有指定权限
        var hasClaim = context.User.Claims.Any(c =>
            c.Type == "Permission" && c.Value == requirement.Permission);

        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// 资源操作授权处理器
/// </summary>
public class ResourceOperationAuthorizationHandler : AuthorizationHandler<ResourceOperationRequirement>
{
    /// <summary>
    /// 处理资源操作要求
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOperationRequirement requirement)
    {
        // 检查用户是否已认证
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // 超级管理员拥有所有权限
        if (context.User.IsInRole(PermissionConstants.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 构造权限字符串：ResourceType.Operation（例如：User.Create）
        var permission = $"{requirement.ResourceType}.{requirement.Operation}";

        // 检查用户是否拥有该资源操作权限
        var hasClaim = context.User.Claims.Any(c =>
            c.Type == "Permission" && c.Value == permission);

        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// 多角色授权处理器
/// </summary>
public class MultiRoleAuthorizationHandler : AuthorizationHandler<MultiRoleRequirement>
{
    /// <summary>
    /// 处理多角色要求
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MultiRoleRequirement requirement)
    {
        // 检查用户是否已认证
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // 超级管理员拥有所有权限
        if (context.User.IsInRole(PermissionConstants.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        bool hasRequiredRoles;

        if (requirement.RequireAll)
        {
            // 需要拥有所有指定角色
            hasRequiredRoles = requirement.Roles.All(role => context.User.IsInRole(role));
        }
        else
        {
            // 只需拥有任意一个指定角色
            hasRequiredRoles = requirement.Roles.Any(role => context.User.IsInRole(role));
        }

        if (hasRequiredRoles)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
