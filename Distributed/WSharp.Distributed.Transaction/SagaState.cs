namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga 执行状态
/// </summary>
public enum SagaState
{
    /// <summary>
    /// Saga 尚未开始
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Saga 正在执行中
    /// </summary>
    Running = 1,

    /// <summary>
    /// Saga 成功完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Saga 失败并正在进行补偿
    /// </summary>
    Compensating = 3,

    /// <summary>
    /// Saga 已成功补偿
    /// </summary>
    Compensated = 4,

    /// <summary>
    /// Saga 补偿失败
    /// </summary>
    CompensationFailed = 5
}
