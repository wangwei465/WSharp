using WSharp.Core.Domain.Events;

namespace WSharp.Core.Domain.Entities;

/// <summary>
/// 聚合根基类，支持领域事件
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey> where TKey : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// 获取领域事件集合（只读）
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => this._domainEvents.AsReadOnly();

    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        this._domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// 移除领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        this._domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// 清除所有领域事件
    /// </summary>
    public void ClearDomainEvents()
    {
        this._domainEvents.Clear();
    }
}
