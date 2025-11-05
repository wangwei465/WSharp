namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga execution context
/// </summary>
public class SagaContext
{
    /// <summary>
    /// Unique saga instance ID
    /// </summary>
    public string SagaId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Saga name/type
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Current saga state
    /// </summary>
    public SagaState State { get; set; } = SagaState.NotStarted;

    /// <summary>
    /// Current step index
    /// </summary>
    public int CurrentStepIndex { get; set; } = -1;

    /// <summary>
    /// Saga data payload
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Error information if saga failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Stack trace of the error
    /// </summary>
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// Saga start time
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Saga completion time
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Set a data value
    /// </summary>
    public void SetData<T>(string key, T value) where T : notnull
    {
        Data[key] = value;
    }

    /// <summary>
    /// Get a data value
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
