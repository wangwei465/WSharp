namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga 步骤抽象基类
/// </summary>
public abstract class SagaStep
{
    /// <summary>
    /// 步骤名称
    /// </summary>
    public abstract string StepName { get; }

    /// <summary>
    /// 执行步骤
    /// </summary>
    public abstract Task ExecuteAsync(SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 补偿步骤（回滚）
    /// </summary>
    public abstract Task CompensateAsync(SagaContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// 带有类型化输入的泛型 Saga 步骤
/// </summary>
public abstract class SagaStep<TInput> : SagaStep where TInput : class
{
    /// <summary>
    /// 使用类型化输入执行步骤
    /// </summary>
    protected abstract Task ExecuteAsync(TInput input, SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用类型化输入补偿步骤
    /// </summary>
    protected abstract Task CompensateAsync(TInput input, SagaContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行步骤
    /// </summary>
    public override async Task ExecuteAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        var input = context.GetData<TInput>("Input");
        if (input == null)
        {
            throw new InvalidOperationException($"在 Saga 上下文中未找到类型为 {typeof(TInput).Name} 的输入数据");
        }

        await ExecuteAsync(input, context, cancellationToken);
    }

    /// <summary>
    /// 补偿步骤
    /// </summary>
    public override async Task CompensateAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        var input = context.GetData<TInput>("Input");
        if (input == null)
        {
            throw new InvalidOperationException($"在 Saga 上下文中未找到类型为 {typeof(TInput).Name} 的输入数据");
        }

        await CompensateAsync(input, context, cancellationToken);
    }
}
