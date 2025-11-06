namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga 编排器接口
/// </summary>
public interface ISagaOrchestrator
{
    /// <summary>
    /// 执行 Saga
    /// </summary>
    Task<SagaContext> ExecuteAsync(SagaContext context, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用输入数据执行 Saga
    /// </summary>
    Task<SagaContext> ExecuteAsync<TInput>(string sagaName, TInput input, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default) where TInput : class;

    /// <summary>
    /// 从当前状态恢复 Saga
    /// </summary>
    Task<SagaContext> ResumeAsync(string sagaId, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// 补偿 Saga（回滚所有已完成的步骤）
    /// </summary>
    Task<SagaContext> CompensateAsync(SagaContext context, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);
}
