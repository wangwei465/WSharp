namespace WSharp.Distributed.Transaction;

/// <summary>
/// 用于持久化 Saga 状态的仓储接口
/// </summary>
public interface ISagaStateRepository
{
    /// <summary>
    /// 保存 Saga 上下文
    /// </summary>
    Task SaveAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取 Saga 上下文
    /// </summary>
    Task<SagaContext?> GetAsync(string sagaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新 Saga 上下文
    /// </summary>
    Task UpdateAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除 Saga 上下文
    /// </summary>
    Task DeleteAsync(string sagaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取处于特定状态的所有 Saga
    /// </summary>
    Task<IEnumerable<SagaContext>> GetByStateAsync(SagaState state, CancellationToken cancellationToken = default);
}
