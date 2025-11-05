namespace WSharp.Core.Domain.Entities;

/// <summary>
/// 审计接口，用于跟踪实体的创建和修改信息
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// 获取或设置创建时间
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// 获取或设置创建人ID
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置最后修改时间
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 获取或设置最后修改人ID
    /// </summary>
    string? UpdatedBy { get; set; }
}

/// <summary>
/// 软删除接口，用于标记实体是否已删除
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// 获取或设置是否已删除
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// 获取或设置删除时间
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 获取或设置删除人ID
    /// </summary>
    string? DeletedBy { get; set; }
}

/// <summary>
/// 完整审计接口（包含软删除）
/// </summary>
public interface IFullAuditable : IAuditable, ISoftDelete
{
}
