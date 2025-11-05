using Microsoft.AspNetCore.Authorization;

namespace WSharp.Authorization;

/// <summary>
/// 权限要求
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 权限名称
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// 初始化权限要求
    /// </summary>
    /// <param name="permission">权限名称</param>
    public PermissionRequirement(string permission)
    {
        this.Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}

/// <summary>
/// 资源操作要求
/// </summary>
public class ResourceOperationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public string ResourceType { get; }

    /// <summary>
    /// 操作
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// 初始化资源操作要求
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="operation">操作</param>
    public ResourceOperationRequirement(string resourceType, string operation)
    {
        this.ResourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
        this.Operation = operation ?? throw new ArgumentNullException(nameof(operation));
    }
}

/// <summary>
/// 多角色要求
/// </summary>
public class MultiRoleRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 角色列表
    /// </summary>
    public IEnumerable<string> Roles { get; }

    /// <summary>
    /// 是否需要所有角色
    /// </summary>
    public bool RequireAll { get; }

    /// <summary>
    /// 初始化多角色要求
    /// </summary>
    /// <param name="roles">角色列表</param>
    /// <param name="requireAll">是否需要所有角色</param>
    public MultiRoleRequirement(IEnumerable<string> roles, bool requireAll = false)
    {
        this.Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        this.RequireAll = requireAll;
    }
}
