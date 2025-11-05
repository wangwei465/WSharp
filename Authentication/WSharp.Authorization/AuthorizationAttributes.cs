using Microsoft.AspNetCore.Authorization;

namespace WSharp.Authorization;

/// <summary>
/// 权限特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 初始化权限特性
    /// </summary>
    /// <param name="permission">权限名称</param>
    public PermissionAttribute(string permission)
    {
        this.Policy = $"Permission:{permission}";
    }
}

/// <summary>
/// 资源操作特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ResourceOperationAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 初始化资源操作特性
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="operation">操作</param>
    public ResourceOperationAttribute(string resourceType, string operation)
    {
        this.Policy = $"Resource:{resourceType}:{operation}";
    }
}

/// <summary>
/// 多角色特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class MultiRoleAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 初始化多角色特性
    /// </summary>
    /// <param name="requireAll">是否需要所有角色</param>
    /// <param name="roles">角色列表</param>
    public MultiRoleAttribute(bool requireAll, params string[] roles)
    {
        var prefix = requireAll ? "AllRoles" : "AnyRole";
        this.Policy = $"{prefix}:{string.Join(",", roles)}";
    }

    /// <summary>
    /// 初始化多角色特性（默认只需任意角色）
    /// </summary>
    /// <param name="roles">角色列表</param>
    public MultiRoleAttribute(params string[] roles) : this(false, roles)
    {
    }
}
