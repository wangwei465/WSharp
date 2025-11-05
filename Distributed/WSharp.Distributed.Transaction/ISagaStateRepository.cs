namespace WSharp.Distributed.Transaction;

/// <summary>
/// Repository for persisting saga state
/// </summary>
public interface ISagaStateRepository
{
    /// <summary>
    /// Save saga context
    /// </summary>
    Task SaveAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get saga context by ID
    /// </summary>
    Task<SagaContext?> GetAsync(string sagaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update saga context
    /// </summary>
    Task UpdateAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete saga context
    /// </summary>
    Task DeleteAsync(string sagaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sagas in a specific state
    /// </summary>
    Task<IEnumerable<SagaContext>> GetByStateAsync(SagaState state, CancellationToken cancellationToken = default);
}
