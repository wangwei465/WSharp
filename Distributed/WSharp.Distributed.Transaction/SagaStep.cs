namespace WSharp.Distributed.Transaction;

/// <summary>
/// Abstract base class for saga steps
/// </summary>
public abstract class SagaStep
{
    /// <summary>
    /// Step name
    /// </summary>
    public abstract string StepName { get; }

    /// <summary>
    /// Execute the step
    /// </summary>
    public abstract Task ExecuteAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compensate the step (rollback)
    /// </summary>
    public abstract Task CompensateAsync(SagaContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic saga step with typed input
/// </summary>
public abstract class SagaStep<TInput> : SagaStep where TInput : class
{
    /// <summary>
    /// Execute the step with typed input
    /// </summary>
    protected abstract Task ExecuteAsync(TInput input, SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compensate the step with typed input
    /// </summary>
    protected abstract Task CompensateAsync(TInput input, SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute the step
    /// </summary>
    public override async Task ExecuteAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        var input = context.GetData<TInput>("Input");
        if (input == null)
        {
            throw new InvalidOperationException($"Input data of type {typeof(TInput).Name} not found in saga context");
        }

        await ExecuteAsync(input, context, cancellationToken);
    }

    /// <summary>
    /// Compensate the step
    /// </summary>
    public override async Task CompensateAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        var input = context.GetData<TInput>("Input");
        if (input == null)
        {
            throw new InvalidOperationException($"Input data of type {typeof(TInput).Name} not found in saga context");
        }

        await CompensateAsync(input, context, cancellationToken);
    }
}
