namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga orchestrator interface
/// </summary>
public interface ISagaOrchestrator
{
    /// <summary>
    /// Execute a saga
    /// </summary>
    Task<SagaContext> ExecuteAsync(SagaContext context, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute a saga with input data
    /// </summary>
    Task<SagaContext> ExecuteAsync<TInput>(string sagaName, TInput input, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default) where TInput : class;

    /// <summary>
    /// Resume a saga from its current state
    /// </summary>
    Task<SagaContext> ResumeAsync(string sagaId, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compensate a saga (rollback all completed steps)
    /// </summary>
    Task<SagaContext> CompensateAsync(SagaContext context, IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default);
}
