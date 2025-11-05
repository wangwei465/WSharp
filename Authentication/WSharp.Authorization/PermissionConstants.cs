namespace WSharp.Authorization;

/// <summary>
/// 权限常量
/// </summary>
public static class PermissionConstants
{
    /// <summary>
    /// 超级管理员角色
    /// </summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>
    /// 管理员角色
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// 用户角色
    /// </summary>
    public const string User = "User";
}

/// <summary>
/// 权限操作
/// </summary>
public static class PermissionOperations
{
    /// <summary>
    /// 创建
    /// </summary>
    public const string Create = "Create";

    /// <summary>
    /// 读取
    /// </summary>
    public const string Read = "Read";

    /// <summary>
    /// 更新
    /// </summary>
    public const string Update = "Update";

    /// <summary>
    /// 删除
    /// </summary>
    public const string Delete = "Delete";

    /// <summary>
    /// 列表
    /// </summary>
    public const string List = "List";

    /// <summary>
    /// 执行
    /// </summary>
    public const string Execute = "Execute";
}

/// <summary>
/// 策略名称
/// </summary>
public static class PolicyNames
{
    /// <summary>
    /// 需要超级管理员
    /// </summary>
    public const string RequireSuperAdmin = "RequireSuperAdmin";

    /// <summary>
    /// 需要管理员
    /// </summary>
    public const string RequireAdmin = "RequireAdmin";

    /// <summary>
    /// 需要用户
    /// </summary>
    public const string RequireUser = "RequireUser";

    /// <summary>
    /// 基于资源的授权
    /// </summary>
    public const string ResourceBased = "ResourceBased";
}
