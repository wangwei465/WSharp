namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga execution states
/// </summary>
public enum SagaState
{
    /// <summary>
    /// Saga has not started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Saga is currently executing
    /// </summary>
    Running = 1,

    /// <summary>
    /// Saga completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Saga failed and is being compensated
    /// </summary>
    Compensating = 3,

    /// <summary>
    /// Saga was compensated successfully
    /// </summary>
    Compensated = 4,

    /// <summary>
    /// Saga compensation failed
    /// </summary>
    CompensationFailed = 5
}
