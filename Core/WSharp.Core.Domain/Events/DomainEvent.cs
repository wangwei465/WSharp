namespace WSharp.Core.Domain.Events;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 获取事件发生时间
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// 获取事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// 初始化 <see cref="DomainEvent"/> 类的新实例
    /// </summary>
    protected DomainEvent()
    {
        this.OccurredOn = DateTime.UtcNow;
    }
}
