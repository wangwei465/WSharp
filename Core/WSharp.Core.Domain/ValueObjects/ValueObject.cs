namespace WSharp.Core.Domain.ValueObjects;

/// <summary>
/// 值对象基类，通过属性值判断相等性
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// 获取用于判断相等性的原子值
    /// </summary>
    /// <returns>原子值集合</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// 判断两个值对象是否相等
    /// </summary>
    /// <param name="other">另一个值对象</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public bool Equals(ValueObject? other)
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

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// 判断两个对象是否相等
    /// </summary>
    /// <param name="obj">另一个对象</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as ValueObject);
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    /// <returns>哈希码</returns>
    public override int GetHashCode()
    {
        return this.GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// 相等运算符
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="right">右操作数</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
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
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
