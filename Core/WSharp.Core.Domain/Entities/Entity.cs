namespace WSharp.Core.Domain.Entities;

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class Entity<TKey> : IEquatable<Entity<TKey>> where TKey : notnull
{
    /// <summary>
    /// 获取或设置实体的唯一标识
    /// </summary>
    public TKey Id { get; protected set; } = default!;

    /// <summary>
    /// 判断两个实体是否相等
    /// </summary>
    /// <param name="other">另一个实体</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public bool Equals(Entity<TKey>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.GetType() != other.GetType())
        {
            return false;
        }

        return EqualityComparer<TKey>.Default.Equals(this.Id, other.Id);
    }

    /// <summary>
    /// 判断两个对象是否相等
    /// </summary>
    /// <param name="obj">另一个对象</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Entity<TKey>);
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    /// <returns>哈希码</returns>
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    /// <summary>
    /// 相等运算符
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="right">右操作数</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// 不相等运算符
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="right">右操作数</param>
    /// <returns>如果不相等则返回 true，否则返回 false</returns>
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !(left == right);
    }
}
