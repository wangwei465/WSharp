namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga 执行上下文
/// </summary>
public class SagaContext
{
    /// <summary>
    /// 唯一的 Saga 实例ID
    /// </summary>
    public string SagaId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Saga 名称/类型
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// 当前 Saga 状态
    /// </summary>
    public SagaState State { get; set; } = SagaState.NotStarted;

    /// <summary>
    /// 当前步骤索引
    /// </summary>
    public int CurrentStepIndex { get; set; } = -1;

    /// <summary>
    /// Saga 数据负载
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Saga 失败时的错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 错误堆栈跟踪
    /// </summary>
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// Saga 开始时间
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Saga 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 设置数据值
    /// </summary>
    public void SetData<T>(string key, T value) where T : notnull
    {
        Data[key] = value;
    }

    /// <summary>
    /// 获取数据值
    /// </summary>
    public T? GetData<T>(string key)
    {
        if (Data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }
}
